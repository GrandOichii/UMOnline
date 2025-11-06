using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class BloodyMaryTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Bloody Mary")
        .Load("../../../../.generated/loadouts/Bloody Mary/Bloody Mary.json")
        .ClearDeck();

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    [InlineData(4, 2)]
    [InlineData(5, 2)]
    public async Task CheckTurnStartTrigger(int handSize, int expectedUnspentActions)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

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
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
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
            .HasUnspentActions(expectedUnspentActions)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}
