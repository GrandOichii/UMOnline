namespace UMCore.Tests.Fighters;

// mostly copied from Sherlock Holmes

public class WinterSoldierTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Winter Soldier")
        .Load("../../../../.generated/loadouts/Winter Soldier/Winter Soldier.json")
        .ClearDeck();

    [Fact]
    public async Task RedRoomEnabled()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Winter Soldier
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

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
            .StringAttrEq("RED_ROOM_ACTIVE", "Y")
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task BaselineAny()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
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
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Winter Soldier")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
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

    [Fact]
    public async Task BaselineNameScoped()
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
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Winter Soldier")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
                        .Value(5)
                        .Versatile()
                        .CanBePlayedBy("Winter Soldier")
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
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Winter Soldier")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
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
                    .Add(new LoadoutCardTemplateBuilder()
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

    [Fact]
    public async Task CantCancelNameScoped()
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
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Winter Soldier")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
                        .Value(5)
                        .Versatile()
                        .CanBePlayedBy("Winter Soldier")
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
                    .Add(new LoadoutCardTemplateBuilder()
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