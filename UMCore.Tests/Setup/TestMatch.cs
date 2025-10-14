using System.Threading.Tasks;
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

    public MatchAsserts Assert()
    {
        return new(this);
    }
}