using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class TheWaywardSistersTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("The Wayward Sisters")
        .Load("../../../../.generated/loadouts/The Wayward Sisters/The Wayward Sisters.json")
        .ClearDeck();

    // TODO test that if one sister is defeated the zone change still applies

    [Fact]
    public async Task MillMain()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(5)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInDeck(5)
            .HasCardZone("CAULDRON")
            .HasCardsInDiscardPile(0)
            .HasCardsInZone("CAULDRON", 5)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .DoesntHaveCardZone("CAULDRON")
            .IsNotWinner();
    }

    [Fact]
    public async Task MillMain_OneSisterDead()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DealDamage("Wayward Sister", 10)
                    .Mill(5)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInDeck(5)
            .HasCardZone("CAULDRON")
            .HasCardsInDiscardPile(0)
            .HasCardsInZone("CAULDRON", 5)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .DoesntHaveCardZone("CAULDRON")
            .IsNotWinner();
    }

    [Fact]
    public async Task MillOpponent()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 1) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(5)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInDeck(10)
            .HasCardZone("CAULDRON")
            .HasCardsInDiscardPile(0)
            .HasCardsInZone("CAULDRON", 0)
            .IsNotWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .DoesntHaveCardZone("CAULDRON")
            .HasCardsInDiscardPile(0)
            .IsWinner();
    }

    [Fact]
    public async Task AttackNoIngredients()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAll()
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
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(1, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInZone("CAULDRON", 1)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("snake", "snake", "snake")]
    [InlineData("bat", "bat", "bat")]
    [InlineData("leg", "leg", "leg")]
    public async Task AttackInvalidIngredients(string card1Ing, string card2Ing, string card3Ing)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(2)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Amount(1)
                        .Versatile()
                        .Value(1)
                        .HasLabel(card1Ing)
                        .Build()
                    )
                    .Add(new CardTemplateBuilder()
                        .Amount(1)
                        .Versatile()
                        .Value(1)
                        .HasLabel(card2Ing)
                        .Build()
                    )
                    .Add(new CardTemplateBuilder()
                        .Amount(1)
                        .Versatile()
                        .Value(1)
                        .HasLabel(card3Ing)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInZone("CAULDRON", 3)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("snake", "snake", "bat", 2)]
    [InlineData("snake", "snake", "leg", 2)]
    [InlineData("bat", "bat", "leg", 2)]
    [InlineData("snake", "bat", "leg", 5)]
    public async Task Attack_ValidIngredients_Decline(string card1Ing, string card2Ing, string card3Ing, int expectedOptionsCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(2)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Assert(a => a
                        .OptionsCount(expectedOptionsCount)
                    )
                    .First()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Amount(1)
                        .Versatile()
                        .Value(1)
                        .HasLabel(card1Ing)
                        .Build()
                    )
                    .Add(new CardTemplateBuilder()
                        .Amount(1)
                        .Versatile()
                        .Value(1)
                        .HasLabel(card2Ing)
                        .Build()
                    )
                    .Add(new CardTemplateBuilder()
                        .Amount(1)
                        .Versatile()
                        .Value(1)
                        .HasLabel(card3Ing)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInZone("CAULDRON", 3)
            .HasUnspentActions(1)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task Attack_Accept_SnakeBat()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DealDamageToAllFightersWithName("Wayward Sister", 1)
                    .Mill(1)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigStringChoices(c => c
                    .Last()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Amount(3)
                        .Versatile()
                        .Value(1)
                        .Amount(3)
                        .HasLabel("snake")
                        .HasLabel("bat")
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInZone("CAULDRON", 0)
            .HasCardsInDiscardPile(2)
            .CombinedFighterHealthEq(16)
            .HasUnspentActions(1)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task Attack_Accept_SnakeLeg()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(4, [0])
            .AddNode(5, [0])
            .AddNode(6, [0])
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [0])                 // Wayward Sister 3
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(1)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .InNodeWithId(0)
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(4)
                )
                .ConfigStringChoices(c => c
                    .Last()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Amount(3)
                        .Versatile()
                        .Value(1)
                        .Amount(3)
                        .HasLabel("snake")
                        .HasLabel("leg")
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
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
            .HasCardsInZone("CAULDRON", 0)
            .HasCardsInDiscardPile(2)
            .HasUnspentActions(1)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task Attack_Accept_BatLeg()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(4, [2])                 // Bar
            .AddNode(5, [2])
            .AddNode(6, [2])
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [1])                 // Wayward Sister 3
            .AddNode(3, [1], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(1)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Last()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .ConfigFighterChoices(c => c
                    .Assert(a => a
                        .OptionsCount(4) // sisters and Foo, NOT Bar
                    )
                    .WithName("Foo")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Amount(3)
                        .Versatile()
                        .Value(0)
                        .Amount(3)
                        .HasLabel("bat")
                        .HasLabel("leg")
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigNodeChoices(c => c
                    .WithId(4)
                )
                .Build(),
            LoadoutTemplateBuilder.FooBar()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasCardsInZone("CAULDRON", 0)
            .HasCardsInDiscardPile(2)
            .HasUnspentActions(1)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(1);
    }

    [Fact]
    public async Task Attack_Accept_SnakeBatLeg()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(4, [2])                 // Bar
            .AddNode(5, [2])
            .AddNode(6, [2])
            .AddNode(0, [0], spawnNumber: 1) // Wayward Sister 1
            .AddNode(1, [0])                 // Wayward Sister 2
            .AddNode(2, [1])                 // Wayward Sister 3
            .AddNode(3, [1], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(3)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Last()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Amount(3)
                        .Versatile()
                        .Value(0)
                        .Amount(4)
                        .HasLabel("bat")
                        .HasLabel("leg")
                        .HasLabel("snake")
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigNodeChoices(c => c
                    .WithId(4)
                )
                .Build(),
            LoadoutTemplateBuilder.FooBar()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasCardsInZone("CAULDRON", 0)
            .HasCardsInDiscardPile(4)
            .HasUnspentActions(2)
            .IsWinner();

        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}
