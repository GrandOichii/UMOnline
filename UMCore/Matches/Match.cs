using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches;

public class Match : IHasData<Match.Data>
{
    public required ILogger? Logger { get; init; }
    public List<Player> Players { get; } = [];
    public int CurPlayerIdx { get; private set; }
    public Map Map { get; }
    public List<MatchCard> Cards { get; }
    public List<Fighter> Fighters { get; }
    public Lua LState { get; }
    public Combat? Combat { get; private set; }
    public Random Random { get; }

    public Match(MapTemplate mapTemplate, string setupScript)
    {
        Map = new(this, mapTemplate);
        Cards = [];
        Fighters = [];
        LState = new();
        Combat = null;
        Random = new();

        LState.DoString(setupScript);
        new MatchScripts(this);
    }

    public Player GetPlayer(int idx) => Players[idx];

    public async Task<Player> AddPlayer(string name, int teamIdx, LoadoutTemplate loadout, IPlayerController controller)
    {
        // TODO check whether a player with the specified loadout already exists

        var player = new Player(this, Players.Count, name, teamIdx, loadout, controller);

        Players.Add(player);

        return player;
    }

    public async Task Run()
    {
        Logger!.LogDebug("Starting match");
        await Setup();

        while (!IsWinnerDetermined())
        {
            var current = CurrentPlayer();
            await current.TakeTurn();
            SetNextPlayer();
        }

    }

    private async Task Setup()
    {
        // TODO

        foreach (var player in Players)
        {
            await player.Setup();
        }
    }

    private void SetNextPlayer()
    {
        CurPlayerIdx = (CurPlayerIdx + 1) % Players.Count;
    }

    public bool IsWinnerDetermined()
    {
        //TODO
        return false;
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

        await Combat.Process();

        Combat = null;
    }

    public Data GetData(Player player)
    {
        return new()
        {
            CurPlayerIdx = CurPlayerIdx,
            Players = [.. Players.Select(p => p.GetData(p))]
        };
    }

    public class Data
    {
        public required int CurPlayerIdx { get; init; }
        public required Player.Data[] Players { get; init; }
    }
}