namespace UMCore.Tests.Mechanics;

public class SchemeTests
{
    [Fact]
    public async Task CantPlaySchemeIfNoSchemeInHand()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
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
                    .Assert(a => a.CantScheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            LoadoutTemplateBuilder.Foo("main")
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
            .IsWinner()
            .HasUnspentActions(2);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CanPlaySchemeOnce()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Scheme()
                    .Assert(a => a.CantScheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
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
            .IsWinner()
            .HasUnspentActions(1);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CanPlaySchemeTwice()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(3)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c
                    .First()
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
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
            .IsWinner()
            .HasUnspentActions(1);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CardDrawSchemeCheck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d.AddCardDrawScheme(1, amount: 10))
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
            .IsWinner()
            .HasCardsInHand(1)
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(8)
            .HasUnspentActions(1);
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CantPlaySchemeWithRequirement()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Assert(a => a.CantScheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Scheme()
                        .InnerScript("""
                        :SchemeRequirement(
                        'Cant play this card',
                        UM.Conditions:False()
                        )
                        """)
                    .Build())
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

    [Fact]
    public async Task CanPlaySchemeWithRequirement()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c.First())
            .Build(),
            new LoadoutTemplateBuilder("main")
                .AddFighter(new FighterTemplateBuilder("main", "main").Build())
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Scheme()
                        .InnerScript("""
                        :SchemeRequirement(
                        'Cant play this card',
                        UM.Conditions:True()
                        )
                        """)
                    .Build())
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
