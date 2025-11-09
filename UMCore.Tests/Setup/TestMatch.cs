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
        Winner = player;
    }
}

public class TestMatchWrapper
{
    public static readonly int MAIN_TEAM = 0;
    public static readonly int OPPONENT_TEAM = 1;

    public TestMatch Match { get; }
    public Exception? Exception { get; private set; } = null;

    public TestMatchWrapper(MatchConfig config, MapTemplate mapTemplate)
    {
        Match = new(config, mapTemplate, File.ReadAllText("../../../../core-new.lua"))
        {
            Logger = null
        };
    }

    public async Task<bool> AddMainPlayer(TestPlayerController controller, LoadoutTemplate loadout)
    {
        return await Match.AddPlayer("Main", MAIN_TEAM, loadout, controller);
    }

    private int _oppCount = 0;
    public async Task<bool> AddOpponent(TestPlayerController controller, LoadoutTemplate loadout)
    {
        return await Match.AddPlayer($"Opp{++_oppCount}", OPPONENT_TEAM, loadout, controller);
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
        node.Fighter.ShouldNotBeNull();
        return new(node.Fighter);
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