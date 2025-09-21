using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches;

public class Match
{
    public required ILogger? Logger { get; init; }
    public List<Player> Players { get; } = [];
    public int CurPlayerIdx { get; private set; }
    public Map Map { get; }
    public List<MatchCard> Cards { get; }
    public List<Fighter> Fighters { get; }
    public Lua LState { get; }

    public Match(MapTemplate mapTemplate, string setupScript)
    {
        Map = new(this, mapTemplate);
        Cards = [];
        Fighters = [];
        LState = new();

        LState.DoString(setupScript);
        new MatchScripts(this);
    }

    public Player GetPlayer(int idx) => Players[idx];

    public async Task<Player> AddPlayer(string name, int teamIdx, IPlayerController controller)
    {
        var player = new Player(this, Players.Count, name, teamIdx, controller);

        Players.Add(player);

        return player;
    }

    public async Task Run()
    {
        System.Console.WriteLine(Logger);
        Logger!.LogDebug("Starting match");
        await Setup();

        while (!IsWinnerDetermined())
        {
            var current = CurrentPlayer();
            await current.TakeTurn();
        }

        SetNextPlayer();
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

    public void AddFighter(Fighter fighter)
    {
        Fighters.Add(fighter);
    }

}