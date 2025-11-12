namespace UMCore.Tests.Fighters;

public class SherlockHolmesTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Sherlock Holmes")
        .Load("../../../../.generated/loadouts/Sherlock Holmes/Sherlock Holmes.json")
        .ClearDeck();

    [Fact]
    public async Task BaselineAny()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(9, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1) // Watson
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Sherlock Holmes")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(5)
                        .Versatile()
                        .CanBePlayedByAny()
                        .Amount(2)

                        .Script("""
                        :AfterCombat(
                        'After combat: Draw 1 card',
                        UM.Effects:Draw(
                        UM.Select:Players()
                        :You()
                        :Build(), 
                        UM.Number:Static(1), 
                        false
                        )
                        )
                        """)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .AddBasicVersatile(3)
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
            .HasCardsInHand(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(2);
    }

    [Theory]
    [InlineData("Sherlock Holmes")]
    [InlineData("Dr. Watson")]
    public async Task BaselineNameScoped(string fighterName)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(9, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1) // Watson
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(fighterName)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(5)
                        .Versatile()
                        .CanBePlayedBy(fighterName)
                        .Amount(2)
                        .Script("""
                        :AfterCombat(
                        'After combat: Draw 1 card',
                        UM.Effects:Draw(
                        UM.Select:Players()
                        :You()
                        :Build(), 
                        UM.Number:Static(1), 
                        false
                        )
                        )
                        """)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .AddBasicVersatile(3)
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
            .HasCardsInHand(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(2);
    }

    [Fact]
    public async Task CanCancelAny()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(9, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1) // Watson
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Sherlock Holmes")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(5)
                        .Versatile()
                        .CanBePlayedByAny()
                        .Amount(2)
                        .Script("""
                        :AfterCombat(
                        'After combat: Draw 1 card',
                        UM.Effects:Draw(
                        UM.Select:Players()
                        :You()
                        :Build(), 
                        UM.Number:Static(1), 
                        false
                        )
                        )
                        """)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Feint()
                        .Versatile()
                        .Value(3)
                        .Amount(1)                    
                        .Build()
                    )
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
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(2);
    }

    [Theory]
    [InlineData("Sherlock Holmes")]
    [InlineData("Dr. Watson")]
    public async Task CantCancelNameScoped(string fighterName)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(9, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1) // Watson
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(fighterName)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(5)
                        .Versatile()
                        .CanBePlayedBy(fighterName)
                        .Amount(2)
                        .Script("""
                        :AfterCombat(
                        'After combat: Draw 1 card',
                        UM.Effects:Draw(
                        UM.Select:Players()
                        :You()
                        :Build(), 
                        UM.Number:Static(1), 
                        false
                        )
                        )
                        """)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Feint()
                        .Versatile()
                        .Value(3)
                        .Amount(1)                    
                        .Build()
                    )
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
            .HasCardsInHand(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(2);
    }
}