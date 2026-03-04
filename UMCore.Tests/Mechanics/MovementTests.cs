namespace UMCore.Tests.Mechanics;

public class MovementTests
{
    [Fact]
    public async Task CantDiscardForBoost()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4 - 5
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2)
            .AddNode(5, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

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
                .ConfigCardChoices(c => c
                    .Assert(a => a.OptionsEmpty())
                    .Nothing()
                )
                .ConfigPathChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(1)
                    .Build()
                )
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Amount(1)
                        .NoBoost()
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
    
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 7)]
    [InlineData(4, 12)]
    [InlineData(5, 20)]
    public async Task LineMovementTests(int fighterMovement, int expectedPathsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4 - 5
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2)
            .AddNode(5, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

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
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 7)]
    [InlineData(4, 12)]
    [InlineData(5, 20)]
    public async Task LineMovementWithBoostTests(int boostValue, int expectedPathsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(5)
            .Build();

        // 0 - 1 - 2 - 3 - 4 - 5
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2)
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c
                    .First()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(0)
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicScheme(boostValue, 10)
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .HasCardsInHand(5)
            .HasCardsInDeck(4)
            .HasCardsInDiscardPile(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 3)]
    [InlineData(3, 4)]
    [InlineData(4, 9)]
    [InlineData(5, 12)]
    public async Task LineMovementWithSidekickTests(int boostValue, int expectedPathsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(5)
            .Build();

        // 0 - 1 - 2 - 3 - 4 - 5
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // hero is here
            .AddNode(1, [0])                 // sidekick is here
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], spawnNumber: 2) // opponent is here
            .AddNode(5, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c
                    .First()
                )
                .ConfigFighterChoices(c => c
                    .WithName("foo1")
                    .WithName("bar1")
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(0)
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("bar1", "bar1")
                    .IsSidekick()
                    .Movement(0)
                    .Build())
                .ConfigDeck(d => d
                    .AddBasicScheme(boostValue, 10)
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .HasCardsInHand(5)
            .HasCardsInDeck(4)
            .HasCardsInDiscardPile(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 7)]
    [InlineData(4, 12)]
    public async Task LineWithSecretPassageMovementTests(int fighterMovement, int expectedPathsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], true, spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0], true)
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

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
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedPathsCount)
                    )
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task LineWithSurroundedTests(int fighterMovement)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .Build();

        // 0 - 1 - 2 - 3 - 4
        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // opponent hero
            .AddNode(1, [0], spawnNumber: 1) // main hero
            .AddNode(2, [0])                 // opponent sidekick
            .AddNode(3, [0])
            .AddNode(4, [0])
            .Connect(0, 1)
            .Connect(1, 2)
            .Connect(2, 3)
            .Connect(3, 4)
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

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
                .ConfigPathChoices(c => c
                    .Assert(a => a.OptionsCount(1))
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("foo1")
                .AddFighter(new FighterTemplateBuilder("foo1", "foo1")
                    .Movement(fighterMovement)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(2))
                .Build(),
            LoadoutTemplateBuilder.FooBar("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasUnspentActions(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}
