using System.Threading.Tasks;
using UMCore.Tests.Asserts;

namespace UMCore.Tests.Setup;

public class TestMatch
{
    public static readonly int MAIN_TEAM = 0;
    public static readonly int OPPONENT_TEAM = 1;

    public Match Match { get; }
    public Exception? Exception { get; private set; } = null;

    public TestMatch(MatchConfig config, MapTemplate mapTemplate)
    {
        Match = new(config, mapTemplate, File.ReadAllText("../../../../core-new.lua"))
        {
            Logger = null
        };


    }

    public async Task AddMainPlayer(TestPlayerController controller, LoadoutTemplate loadout)
    {
        await Match.AddPlayer("Main", MAIN_TEAM, loadout, controller);
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
}