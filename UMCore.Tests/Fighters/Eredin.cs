using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class EredinTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Eredin")
        .Load("../../../../.generated/loadouts/Eredin/Eredin.json")
        .ClearDeck();

    // TODO

    // [Fact]
    // public async Task CheckInitialRage()
    // {
    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .InitialHandSize(0)
    //         .ActionsPerTurn(2)
    //         .Build();

    //     var mapTemplate = new MapTemplateBuilder()
    //         .AddNode(0, [0], spawnNumber: 1) // Eredin
    //         .AddNode(1, [0])                 // Wiglaf
    //         .AddNode(2, [0], spawnNumber: 2) // Foo
    //         .ConnectAllAsLine()
    //         .Build();

    //     var match = new TestMatchWrapper(
    //         config,
    //         mapTemplate
    //     );

    //     await match.AddMainPlayer(
    //         new TestPlayerControllerBuilder()
    //             .ConfigActions(a => a
    //                 .DeclareWinner()
    //                 .CrashMatch()
    //             )
    //             .ConfigNodeChoices(c => c
    //                 .WithId(1)
    //             )
    //             .Build(),
    //         GetLoadoutBuilder()
    //             .ConfigDeck(d => d
    //                 .AddBasicVersatile(5)
    //             )
    //             .Build()
    //     );
    //     await match.AddOpponent(
    //         new TestPlayerControllerBuilder()
    //             .Build(),
    //         new LoadoutTemplateBuilder("Foo")
    //             .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
    //                 .Build()
    //             )
    //             // .ConfigDeck(d => d
    //             //     .AddBasicVersatile(3)
    //             // )
    //             .Build()
    //     );

    //     // Act
    //     await match.Run();

    //     // Assert
    //     match.Assert()
    //         .CrashedIntentionally();

    //     match.AssertPlayer(0)
    //         .SetupCalled()
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();
    // }
}
