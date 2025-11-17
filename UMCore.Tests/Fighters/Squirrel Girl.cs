using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class SquirrelGirlTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Squirrel Girl")
        .Load("../../../../.generated/loadouts/Squirrel Girl/Squirrel Girl.json")
        .ClearDeck();

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task TurnStart_AllDefeated(int squirrelNodeId)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Squirrel Girl
            .AddNode(1, [0])                 // Squirrel
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
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
                .ConfigNodeChoices(c => c
                    .AssertOptionsEquivalent([1, 2])
                    .WithId(squirrelNodeId)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .Build()
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

        match.AssertNode(squirrelNodeId)
            .HasFighterWithName("Squirrel");
    }

    [Fact]
    public async Task TurnStart_AllUndefeated()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Squirrel Girl + Squirrel x 4
            .AddNode(1, [0])                 // Squirrel x 4
            .AddNode(2, [0], spawnNumber: 1) // Foo
            .ConnectAll()
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
                // .ConfigNodeChoices(c => c
                //     .AssertOptionsEquivalent([2])
                //     .WithId(2)
                // )
                .ConfigFighterChoices(c => c
                    .FirstSmallInNodeWithId("Squirrel", 1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 0)
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 0)
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 0)
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 0)
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Squirrel", 1)
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c.First())
                .ConfigPathChoices(c => c.First())
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .Build()
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

        match.AssertNode(0)
            .FightersCount(5);
        match.AssertNode(1)
            .FightersCount(3);
        match.AssertNode(2)
            .FightersCount(2)
            .HasFighterWithName("Foo")
            .HasFighterWithName("Squirrel");
    }
}
