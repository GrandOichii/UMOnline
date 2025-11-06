using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class BuffyTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Buffy")
        .Load("../../../../.generated/loadouts/Buffy/Buffy.json")
        .ClearDeck();

    [Theory]
    [InlineData("Xander", "Giles")]
    [InlineData("Giles", "Xander")]
    public async Task ChooseSidekick(string chosenSidekick, string otherSidekick)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Buffy
            .AddNode(1, [0])                 // Buffy's sidekick
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Choose(chosenSidekick)
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .HasFighterWithName("Buffy")
            .HasFighterWithName(chosenSidekick)
            .DoesntHaveFighterWithName(otherSidekick)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task BuffyCanMoveOverOpposing()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])                 // Giles
            .AddNode(0, [0], spawnNumber: 1) // Buffy
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .AddNode(3, [0])
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigStringChoices(c => c
                    .Choose("Giles")
                )
                .ConfigActions(a => a
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigFighterChoices(c => c
                    .WithName("Buffy")
                    .First()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .CanReachNodeWithId(3)
                    )
                    .First()
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
    
    [Theory]
    [InlineData("Xander")]
    [InlineData("Giles")]
    public async Task SidekickCantMoveOverOpposing(string sidekickName)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Buffy
            .AddNode(1, [0])                 // Buffy's sidekick
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .AddNode(3, [0])
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigStringChoices(c => c
                    .Choose(sidekickName)
                )
                .ConfigActions(a => a
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigFighterChoices(c => c
                    .WithName(sidekickName)
                    .First()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .CantReachNodeWithId(3)
                    )
                    .First()
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
    // TODO can move over opponents
}
