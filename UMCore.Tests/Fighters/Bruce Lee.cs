using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class BruceLeeTests
{

    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Bruce Lee")
        .Load("../../../../.generated/loadouts/Bruce Lee/Bruce Lee.json")
        .ClearDeck();

    [Fact]
    public async Task DenyEndOfTurnTrigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0])
            .AddNode(1, [0], spawnNumber: 1)
            .AddNode(2, [0])
            .AddNode(3, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .NTimes(3, nc => nc.First()) // move Bruce Lee
                )
                .ConfigStringChoices(c => c
                    .No()
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DeclareWinner()
                    .CrashMatch()
                )
                .Build(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

    [Fact]
    public async Task AcceptEndOfTurnTrigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0])
            .AddNode(1, [0], spawnNumber: 1)
            .AddNode(2, [0])
            .AddNode(3, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .NTimes(3, nc => nc.First()) // move Bruce Lee
                    .NTimes(1, nc => nc.First()) // move Bruce Lee
                )
                .ConfigStringChoices(c => c
                    .Yes()
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DeclareWinner()
                    .CrashMatch()
                )
                .Build(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

}
