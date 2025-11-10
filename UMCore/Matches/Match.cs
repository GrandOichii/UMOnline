using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Effects;
using UMCore.Matches.Fighters;
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
    public Movement? CurrentMovement { get; private set; }

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
        CurrentMovement = null;

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
        // TODO! teams
        var activePlayers = Players.Where(p => p.GetActiveFighters().Any()).ToList();
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
            if (p.Loadout.CantBePlayedWith.Contains(loadout.Name))
            {
                return false;
            }
            if (loadout.CantBePlayedWith.Contains(p.Loadout.Name))
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

    private void ExecuteGameStartEffects()
    {
        for (int i = 0; i < Players.Count; ++i)
        {
            var player = Players[(CurPlayerIdx + i) % Players.Count];
            player.ExecuteGameStartEffects();
        }
    }

    public List<MapNode> GetAdditionalConnectedNodesFor(Fighter fighter, MapNode node)
    {
        List<MapNode> result = [];
        foreach (var f in Fighters)
        {
            foreach (var conn in f.MovementNodeConnections)
            {
                result.AddRange(conn.GetConnectedNodesFor(fighter, node));
            }
        }
        return result;
    }

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
            await player.CreateDeck();
        }

        Logs.Public("Match started!");
        ExecuteGameStartEffects();

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
            await player.CreateFighters();
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

        player.TurnHistory.RecordAttack(Combat);
        // await Combat.Winner.ExecuteOnWonCombatEffects();
        await Combat.GetLoser()!.ExecuteOnLostCombatEffects();
        Combat = null;
        // TODO add combat event
    }

    public IEnumerable<FighterPredicateEffect> GetOnAttackEffectsFor(Fighter fighter)
    {
        return Fighters.SelectMany(f => f.OnAttackEffects.Where(e => e.Accepts(fighter)));
    }

    public void ExecuteOnFighterDefeatEffects(Fighter fighter)
    {
        foreach (var f in Fighters)
        {
            foreach (var e in f.OnFighterDefeatEffects)
            {
                if (!e.Accepts(fighter)) continue;
                e.Execute();
            }
        }
    }

    public async Task ExecuteOnMoveEffects(Fighter fighter, MapNode fromNode, MapNode? toNode)
    {
        var effects = GetAliveFighters().SelectMany(f => f.OnMoveEffects);
        // TODO order effects (bad, doesnt have a fighter predicate)

        foreach (var effect in effects)
        {
            effect.Execute(fighter, fromNode, toNode);
        }
    }

    // public IEnumerable<(Fighter, EffectCollection)> GetAfterSchemeEffectsFor(Fighter fighter)
    // {
    //     return GetAliveFighters().SelectMany(f => f.AfterSchemeEffects
    //         .Where(e => e.AcceptsFighter(f, fighter))
    //         .Select(e => (f, e))
    //     );
    // }

    public IEnumerable<(Fighter, EffectCollection)> GetEffectCollectionThatAccepts(Fighter fighter, Func<Fighter, List<EffectCollection>> extractor)
    {
        return GetAliveFighters().SelectMany(f => extractor(f)
            .Where(e => e.AcceptsFighter(f, fighter))
            .Select(e => (f, e))
        );
    }

    public IEnumerable<ManoeuvreValueModifier> GetManoeuvreValueModifiersFor(Fighter fighter)
    {
        return GetAliveFighters().SelectMany(f => f.ManoeuvreValueMods.Where(e => e.Accepts(fighter)));
    }

    public IEnumerable<CombatResolutionEffect> GetOnLostCombatEffectsFor(Player player)
    {
        // TODO this really needs a player predicate if players ever need to order effects
        return GetAliveFighters().SelectMany(f => f.OnLostCombatEffects);
    }

    // public IEnumerable<T> GetFighterEffects<T>(Func<Fighter, T> extractor)
    // {
    //     return GetAliveFighters().SelectMany(f => f.ManoeuvreValueMods.Where(e => e.Accepts(fighter)));

    // }

    public Movement SetCurrentMovement(Movement movement)
    {
        if (CurrentMovement is not null)
        {
            // TODO may need to remove this
            throw new MatchException($"Tried to call {SetCurrentMovement} while the active movement hasn't resolved");
        }
        CurrentMovement = movement;
        return movement;
    }

    public void FinishMovement()
    {
        CurrentMovement = null;
    }

    public int ModifyDamage(Fighter damageTo, bool isCombatDamage, int amount)
    {
        var modifiers = Fighters.SelectMany(f => f.DamageModifiers);
        foreach (var mod in modifiers)
            amount = mod.Modify(damageTo, isCombatDamage, amount);
        return amount;
    }

    public IEnumerable<Fighter> GetAliveFighters()
    {
        return Players.SelectMany(p => p.GetAliveFighters());
    }

    public async Task ExecuteAfterMovementEffects()
    {
        if (CurrentMovement is null)
            throw new MatchException($"Called {nameof(ExecuteAfterMovementEffects)} with no {nameof(CurrentMovement)}");
        var fighter = CurrentMovement.Fighter;

        var fighters = GetAliveFighters();
        var effects = GetEffectCollectionThatAccepts(fighter, f => f.AfterMovementEffects);
        // TODO order effects

        foreach (var (source, effect) in effects)
        {
            effect.Execute(source, new(fighter));
        }
    }
    
    public async Task<bool> ReplaceBoostedMovement()
    {
        if (CurrentMovement is null)
            throw new MatchException($"Called {nameof(ReplaceBoostedMovement)} with no {nameof(CurrentMovement)}");

        var replacers = Fighters.SelectMany(f => f.BoostedMovementReplacers);
        // TODO order replacers
        foreach (var replacer in replacers)
        {
            var result = replacer.TryReplace(CurrentMovement);
            if (result) return true;
        }
        return false;
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