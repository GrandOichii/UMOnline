using System.Text.Json;
using Xunit.Sdk;

namespace UMCore.Tests.Fighters;

public class SinbadTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Sinbad")
        .Load("../../../../.generated/loadouts/Sinbad/Sinbad.json")
        .ClearDeck();

    [Theory]
    [InlineData(0, 0, 5)]
    [InlineData(1, 0, 7)]
    [InlineData(2, 0, 9)]
    [InlineData(0, 1, 5)]
    [InlineData(1, 1, 7)]
    [InlineData(2, 1, 9)]
    [InlineData(0, 2, 5)]
    [InlineData(1, 2, 7)]
    [InlineData(2, 2, 9)]
    public async Task ManoeuvreValue(int voyageCardsInDiscard, int nonVoyageCardsInDiscard, int expectedNodeOptionsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            // .ManoeuvreDrawAmount(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0])
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 1)
            .AddNode(5, [0])
            .AddNode(6, [0])
            .AddNode(7, [0])
            .AddNode(8, [0])

            .AddNode(9, [0])
            .AddNode(10, [0])
            .AddNode(11, [0])
            .AddNode(12, [0])
            .AddNode(13, [0]) // porter
            .AddNode(14, [0])
            .AddNode(15, [0])
            .AddNode(16, [0])
            .AddNode(17, [0])

            .AddNode(18, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(voyageCardsInDiscard + nonVoyageCardsInDiscard)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Sinbad")
                    .WithName("The Porter")
                )
                .ConfigNodeChoices(c => c
                    .WithId(13)
                    .AssertOptionsHasLength(expectedNodeOptionsCount)
                    .First()
                    .AssertOptionsHasLength(expectedNodeOptionsCount)
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(1, labels: ["voyage"], amount: voyageCardsInDiscard)
                )
                .ConfigDeck(d => d
                    .AddBasicAttack(1, labels: [], amount: nonVoyageCardsInDiscard)
                )
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
}
