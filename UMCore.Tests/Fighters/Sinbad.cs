using System.Text.Json;
using Xunit.Sdk;

namespace UMCore.Tests.Fighters;

public class SinbadTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Sinbad")
        .Load("../../../../.generated/loadouts/Sinbad/Sinbad.json")
        .ClearDeck();

    [Theory]
    [InlineData(0, 0, 2)]
    [InlineData(1, 0, 3)]
    [InlineData(2, 0, 4)]
    [InlineData(0, 1, 2)]
    [InlineData(1, 1, 3)]
    [InlineData(2, 1, 4)]
    [InlineData(0, 2, 2)]
    [InlineData(1, 2, 3)]
    [InlineData(2, 2, 4)]
    public async Task ManoeuvreValue(int voyageCardsInDiscard, int nonVoyageCardsInDiscard, int expectedMovementValue)
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
                .ConfigFighterChoices(c => c
                    .WithName("Sinbad")
                    .WithName("The Porter")
                )
                .ConfigNodeChoices(c => c
                    .WithId(13)
                    .NTimes(expectedMovementValue, nc => nc.First())
                    .NTimes(expectedMovementValue, nc => nc.First())
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
