using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class LittleRedTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Little Red Riding Hood")
        .Load("../../../../.generated/loadouts/Little Red Riding Hood/Little Red Riding Hood.json")
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
    //         .AddNode(0, [0], spawnNumber: 1) // Little Red Riding Hood
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
    //             .Build()
    //     );
    //     await match.AddOpponent(
    //         TestPlayerControllerBuilder.Crasher(),
    //         LoadoutTemplateBuilder.Foo()
    //     );

    //     // Act
    //     await match.Run();

    //     // Assert
    //     match.Assert()
    //         .CrashedIntentionally();

    //     match.AssertPlayer(0)
    //         .SetupCalled()
    //         .IntAttrEq("RAGE", 1)
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();
    // }
}
