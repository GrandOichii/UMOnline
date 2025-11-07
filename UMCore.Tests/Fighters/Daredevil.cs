using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class DaredevilTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Daredevil")
        .Load("../../../../.generated/loadouts/Daredevil/Daredevil.json")
        .ClearDeck();

    [Theory]
    [InlineData(6)]
    [InlineData(5)]
    [InlineData(4)]
    public async Task BaselineAttack(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5, boost: 3, amount: 10)
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
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicDefense(3, amount: 10)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(5 - 3);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task BlindBoostAttack_Decline(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                    .First()
                )
                .ConfigStringChoices(c => c
                    .No()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5, boost: 3, amount: 10)
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
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicDefense(3)
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
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(10 - handSize)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(5 - 3);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task BlindBoostAttack_Accept(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                    .First()
                )
                .ConfigStringChoices(c => c
                    .Yes()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5, boost: 3, amount: 10)
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
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicDefense(3)
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
            .HasCardsInDiscardPile(2)
            .HasCardsInDeck(10 - handSize - 1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(5 - 3 + 3);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task BlindBoostAttack_Accept_NoCardsInDeck(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                    .First()
                )
                .ConfigStringChoices(c => c
                    .Yes()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5, boost: 3, amount: handSize)
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
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicDefense(3)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(5 - 3);
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task BlindBoostDefense_Decline(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                .ConfigStringChoices(c => c
                    .No()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(3, boost: 3, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
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
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(10 - handSize)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Daredevil")
            .HasDamage(5 - 3);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task BlindBoostDefense_Accept(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                .ConfigStringChoices(c => c
                    .Yes()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(3, boost: 3, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
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
            .HasCardsInDiscardPile(2)
            .HasCardsInDeck(10 - handSize - 1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Daredevil")
            .IsAtFullHealth();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task BlindBoostDefense_Accept_NoCardsInDeck(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                .ConfigStringChoices(c => c
                    .Yes()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(3, boost: 3, amount: handSize)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Daredevil")
            .HasDamage(5 - 3);
    }
    
    [Theory]
    [InlineData(3)]
    // [InlineData(2)]
    // [InlineData(1)]
    public async Task CantBlindBoost_NoDefense(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Daredevil
            .AddNode(1, [0], spawnNumber: 2) // Foo
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
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(3, boost: 3, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
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
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(10 - handSize)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Daredevil")
            .HasDamage(5);
    }
}
