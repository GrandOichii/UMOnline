namespace UMCore.Tests.Mechanics;

public class ExhaustionTests
{
    [Fact]
    public async Task ManoeuvreExhaust()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .ExhaustDamage(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var heroKey = "hero";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c.First())
                .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("hero", heroKey)
                    .Health(10)
                    .Build()
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
            .HasUnspentActions(1)
            .HasCardsInHand(0)
            .HasCardsInDeck(0)
            .HasCardsInDiscardPile(0)
            .IsWinner();
        match.AssertFighter(heroKey)
            .HasHealth(8)
            .IsAlive();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CardDrawSchemeExhaust()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .ExhaustDamage(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var heroKey = "main";
        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", heroKey).Build())
                .ConfigDeck(d => d.AddCardDrawScheme(1, amount: 1))
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
            .HasUnspentActions(1)
            .HasCardsInHand(0)
            .HasCardsInDeck(0)
            .HasCardsInDiscardPile(1)
            .IsWinner();
        match.AssertFighter(heroKey)
            .HasDamage(2)
            .IsAlive();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}
