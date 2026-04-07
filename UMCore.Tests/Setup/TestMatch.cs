using System.Threading.Tasks;
using Shouldly;
using UMCore.Matches.Players;
using UMCore.Tests.Asserts;

namespace UMCore.Tests.Setup;

public class TestMatch(MatchConfig config, MapTemplate mapTemplate, string setupScript) 
    : Match(config, mapTemplate, setupScript)
{
    public void SetWinner(Player player)
    {
        WinnerTeamIdx = player.TeamIdx;
    }
}

public class TestMatchWrapper
{
    public static readonly int MAIN_TEAM = 0;
    public static readonly int OPPONENT_TEAM = 1;

    public TestMatch Match { get; }
    public Exception? Exception { get; private set; } = null;
    private readonly Dictionary<string, IPlayerController> _controllers;
    public QueuedPlayerCollection Players { get; }

    public TestMatchWrapper(MatchConfig config, MapTemplate mapTemplate)
    {
        Match = new(config, mapTemplate, File.ReadAllText("../../../../core.lua"))
        {
            Logger = null
        };
        Players = new(config);
        _controllers = [];
    }

    private void AddPlayer(string name, int teamIdx, TestPlayerController controller, LoadoutTemplate loadout)
    {
        Players.AddPlayer(name, teamIdx, loadout);
        _controllers.Add(name, controller);
    }

    private int _mainCount = 0;
    public async Task AddMainPlayer(TestPlayerController controller, LoadoutTemplate loadout)
    {
        AddPlayer($"Main{++_mainCount}", MAIN_TEAM, controller, loadout);
    }

    private int _oppCount = 0;
    public async Task AddOpponent(TestPlayerController controller, LoadoutTemplate loadout)
    {
        AddPlayer($"Opp{++_oppCount}", OPPONENT_TEAM, controller, loadout);
    }

    public bool CanStart()
    {
        return string.IsNullOrEmpty(Players.CanRun());
    }

    public void SetTokenAmount(string tokenName, int amount)
    {
        var token = Match.Tokens.Get(tokenName);
        token.SetAmount(amount);
    }

    public async Task Run()
    {
        try
        {
            await Match.AddPlayers(Players, _controllers);
            await Match.Run();
        }
        catch (Exception e)
        {
            Exception = e;
        }
    }

    public PlayerAsserts AssertPlayer(int playerIdx)
    {
        return new(Match.GetPlayer(playerIdx));
    }

    public FighterAsserts AssertFighter(string fighterKey)
    {
        var fighter = Match.Fighters.Single(f => f.Template.Key == fighterKey);
        return new(fighter);
    }

    public FighterAsserts AssertFighterInNode(int nodeId)
    {
        var node = Match.Map.Nodes.First(n => n.Id == nodeId);
        var fighters = node.GetFighters();
        var fighter = fighters.Single();
        return new(fighter);
    }

    public FighterAsserts AssertFighterInNode(int nodeId, string fighterKey)
    {
        var node = Match.Map.Nodes.First(n => n.Id == nodeId);
        var fighters = node.GetFighters();
        var fighter = fighters.Single(f => f.Template.Key == fighterKey);
        return new(fighter);
    }

    public MultipleFighterAsserts AssertAllFightersInNode(int nodeId, string fighterName)
    {
        var node = Match.Map.Nodes.First(n => n.Id == nodeId);
        var fighters = node.GetFighters().Where(f => f.Name == fighterName);

        return new([.. fighters]);
    }

    public MultipleFighterAsserts AssertAllFighters()
    {
        return new(Match.Fighters);
    }

    public MapNodeAsserts AssertNode(int id)
    {
        var node = Match.Map.Nodes.Single(n => n.Id == id);
        return new(node);
    }

    public TokenAsserts AssertToken(string tokenName)
    {
        var token = Match.Tokens.Get(tokenName);
        return new(token);
    }

    public MatchAsserts Assert()
    {
        return new(this);
    }
}