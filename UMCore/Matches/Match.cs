using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Templates;

namespace UMCore.Matches;

public class Match : IHasData<Match.Data>, IHasSetupData<Match.SetupData>
{
    public MatchConfig Config { get; }
    public required ILogger? Logger { get; init; }
    public List<Player> Players { get; } = [];
    public int CurPlayerIdx { get; private set; }
    public Map Map { get; }
    public List<MatchCard> Cards { get; }
    public List<Fighter> Fighters { get; }
    public Lua LState { get; }
    public Combat? Combat { get; private set; }
    public Random Random { get; }
    public LogsManager Logs { get; }
    public EventsManager Events { get; }
    public TokenManager Tokens { get; }
    public Player? Winner { get; protected set; }

    public Dictionary<int, List<Player>> Teams { get; }

    public Match(MatchConfig config, MapTemplate mapTemplate, string setupScript)
    {
        Config = config;
        Map = new(this, mapTemplate);
        Cards = [];
        Fighters = [];
        LState = new();
        Combat = null;
        Logs = new(this);
        Events = new(this);
        Tokens = new(this);
        Winner = null;
        Teams = [];

        Random = new();
        if (!Config.RandomMatch)
        {
            Random = new(Config.Seed);
        }

        LState.DoString(setupScript);
        new MatchScripts(this);
    }

    public bool CanRun()
    {
        if (Teams.Count <= 1) return false;
        var pCount = Teams[0].Count;
        return Teams.Values.All(t => t.Count == pCount);
    }

    public bool CheckForWinners()
    {
        var activePlayers = Players.Where(p => p.GetAliveHeroes().Any()).ToList();
        if (activePlayers.Count > 1) return false;

        Winner = activePlayers[0];
        return IsWinnerDetermined();
    }

    #region Player management

    public Player GetPlayer(int idx) => Players[idx];

    public async Task<bool> AddPlayer(string name, int teamIdx, LoadoutTemplate loadout, IPlayerController controller)
    {
        foreach (var p in Players)
        {
            if (p.Loadout.Name == loadout.Name)
            {
                return false;
            }
        }

        var team = GetTeam(teamIdx);
        if (team.Count >= Config.TeamSize)
        {
            return false;
        }

        var player = new Player(this, Players.Count, name, teamIdx, loadout, new SafePlayerController(controller));

        team.Add(player);
        Players.Add(player);

        return true;
    }

    public List<Player> GetTeam(int teamIdx)
    {
        if (!Teams.TryGetValue(teamIdx, out var value))
        {
            value = [];
            Teams[teamIdx] = value;
        }
        return value;
    }

    public bool AreInSameTeam(Player p1, Player p2)
    {
        var team = GetTeam(p1.TeamIdx);
        return team.Contains(p2);
    }

    #endregion

    public async Task Run()
    {
        if (!CanRun())
        {
            throw new MatchException("Cant run match");
        }
        Logger?.LogDebug("Starting match");
        await Setup();

        CurPlayerIdx = Config.FirstPlayerIdx;
        if (Config.RandomFirstPlayer)
            CurPlayerIdx = Random.Next(Players.Count);

        for (int i = 0; i < Players.Count; ++i)
        {
            var player = Players[(CurPlayerIdx + i) % Players.Count];
            await player.InitialPlaceFighters(i + 1);
        }

        Logs.Public("Match started!");

        while (!IsWinnerDetermined())
        {
            var current = CurrentPlayer();
            await current.TakeTurn();
            SetNextPlayer();
        }

        Logger?.LogDebug("Match ended, winner: {PlayerLogName}", Winner!.LogName);
        Logs.Public($"Match ended! Winner is: {Winner!.FormattedLogName}");
        await UpdateClients();
    }

    private async Task Setup()
    {
        foreach (var player in Players)
        {
            await player.Setup();
        }
        
        var setupData = GetSetupData();
        foreach (var player in Players)
        {
            await player.Controller.Setup(player, setupData);
        }
    }

    private void SetNextPlayer()
    {
        CurPlayerIdx = (CurPlayerIdx + 1) % Players.Count;
    }

    public bool IsWinnerDetermined()
    {
        return Winner is not null;
    }

    public Player CurrentPlayer()
    {
        return Players[CurPlayerIdx];
    }

    public int AddCard(MatchCard card)
    {
        var result = Cards.Count;
        Cards.Add(card);
        return result;
    }

    public int AddFighter(Fighter fighter)
    {
        var result = Fighters.Count;
        Fighters.Add(fighter);
        return result;
    }

    public async Task ProcessAttack(Player player, AvailableAttack attack)
    {
        Logger?.LogDebug("Processing attack from player {PlayerLogName}: {FighterLogName} -> {TargetLogName} [{CardLogName}]", player.LogName, attack.Fighter.LogName, attack.Target.LogName, attack.AttackCard.LogName);
        Combat = new(this, attack);
        Logs.Public($"Player {player.FormattedLogName} attacks {attack.Target.FormattedLogName} with {attack.Fighter.FormattedLogName}");

        await Combat.Process();
        if (IsWinnerDetermined()) return;

        Combat = null;

        // TODO add combat event
    }

    public IEnumerable<FighterPredicateEffect> GetOnAttackEffectsFor(Fighter fighter)
    {
        foreach (var f in Fighters)
        {
            foreach (var e in f.OnAttackEffects)
            {
                if (!e.Accepts(fighter)) continue;
                yield return e;
            }
        }
    }

    public IEnumerable<FighterPredicateEffect> GetAfterAttackEffectsFor(Fighter fighter)
    {
        foreach (var f in Fighters)
        {
            foreach (var e in f.AfterAttackEffects)
            {
                if (!e.Accepts(fighter)) continue;
                yield return e;
            }
        }
    }

    public IEnumerable<ManoeuvreValueModifier> GetManoeuvreValueModifiersFor(Fighter fighter)
    {
        foreach (var f in Fighters)
        {
            foreach (var mod in f.ManoeuvreValueMods)
            {
                if (!mod.Accepts(fighter)) continue;
                yield return mod;
            }
        }
    }

    public Data GetData(Player player)
    {
        return new()
        {
            CurPlayerIdx = CurPlayerIdx,
            Players = [.. Players.Select(p => p.GetData(player))],
            Map = Map.GetData(player),
            Combat = Combat?.GetData(player),
        };
    }

    public async Task UpdateClients()
    {
        foreach (var player in Players)
        {
            await player.Controller.Update(player);
        }
    }

    public SetupData GetSetupData()
    {
        return new()
        {
            Config = Config,
            Players = [.. Players.Select(p => p.GetSetupData())],
        };
    }

    public class Data
    {
        public required int CurPlayerIdx { get; init; }
        public required Player.Data[] Players { get; init; }
        public required Map.Data Map { get; init; }
        public required Combat.Data? Combat { get; init; }
    }

    public class SetupData
    {
        public required Player.SetupData[] Players { get; init; }
        public required MatchConfig Config { get; init; }
    }
}