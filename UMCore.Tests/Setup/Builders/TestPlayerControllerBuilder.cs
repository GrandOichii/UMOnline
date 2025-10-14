namespace UMCore.Tests.Setup.Builders;

public class TestPlayerControllerBuilder
{
    // static controllers
    private readonly TestPlayerControllerActions _actions = new();

    public static TestPlayerController AutoPass()
    {
        var result = new TestPlayerController();
        // TODO
        return result;
    }

    public TestPlayerControllerBuilder ConfigActions(Action<TestPlayerControllerActions> actions)
    {
        actions(_actions);
        return this;
    }

    public TestPlayerController Build()
    {
        return new TestPlayerController()
        {
            Actions = _actions.Queue
        };
    }
}

public class TestPlayerControllerActions
{
    public Queue<TestPlayerController.PlayerAction> Queue { get; } = [];

    private TestPlayerControllerActions Enqueue(TestPlayerController.PlayerAction action)
    {
        Queue.Enqueue(action);
        return this;
    }

    public TestPlayerControllerActions DeclareWinner()
    {
        return Enqueue((match, player, options) =>
        {
            match.SetWinner(player);
            return (TestPlayerController.NEXT_ACTION, true);
        });
    }

    public TestPlayerControllerActions CrashMatch()
    {
        return Enqueue((match, player, options) =>
        {
            throw new Exception("Requested crash from TestPlayerController");
        });
    }
}