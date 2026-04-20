namespace UMCore.Tests.Core;

public class GainActionsTests
{
    [Fact]
    public async Task ShouldGain1Action()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(1)
            .Build();
        
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var loader = new ScriptLoader();

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("l1")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Scheme()
                        .Script(loader.Get("GainActionScheme"))
                        .Build()
                    )
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
            .HasUnspentActions(1)
            .HasCardsInDeck(0)
            .HasCardsInDiscardPile(1);
    }
}