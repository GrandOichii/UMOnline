using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class AchillesTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Achilles")
        .Load("../../../../.generated/loadouts/Achilles/Achilles.json")
        .ClearDeck();

    [Fact]
    public async Task PatroclusDefeat()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Achilles
            .AddNode(1, [0])                 // Patroclus
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DealDamage("Patroclus", 6)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
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
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Patroclus")
            .IsDead();
    }

    [Fact]
    public async Task BaselineAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(3)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])                 // Patroclus
            .AddNode(0, [0], spawnNumber: 1) // Achilles
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
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Achilles")
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(5, amount: 10)
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
                    .AddBasicDefense(5)
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
            .HasCardsInHand(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .IsAtFullHealth();
    }

    [Fact]
    public async Task AttackWithDefeatedPatroclus()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(3)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])                 // Patroclus
            .AddNode(0, [0], spawnNumber: 1) // Achilles
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
                    .DealDamage("Patroclus", 6)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Achilles")
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(5, amount: 10)
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
                    .AddBasicDefense(5)
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
    public async Task BaselineDefense()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(3)
            .ActionsPerTurn(2)
            .FirstPlayer(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])                 // Patroclus
            .AddNode(0, [0], spawnNumber: 2) // Achilles
            .AddNode(2, [0], spawnNumber: 1) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicDefense(3, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName("Achilles")
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .AddBasicAttack(5, amount: 10)
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
            .HasCardsInHand(2)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertFighter("Achilles")
            .HasDamage(2);
    }

    [Fact]
    public async Task DefenseWithDefeatedPatroclus_Lose()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(3)
            .ActionsPerTurn(2)
            .FirstPlayer(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])                 // Patroclus
            .AddNode(0, [0], spawnNumber: 2) // Achilles
            .AddNode(2, [0], spawnNumber: 1) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicDefense(3, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DealDamage("Patroclus", 6)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName("Achilles")
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .AddBasicAttack(5, amount: 10)
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
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertFighter("Achilles")
            .HasDamage(2);
    }

    [Fact]
    public async Task DefenseWithDefeatedPatroclus_Win()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(3)
            .ActionsPerTurn(2)
            .FirstPlayer(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0])                 // Patroclus
            .AddNode(0, [0], spawnNumber: 2) // Achilles
            .AddNode(2, [0], spawnNumber: 1) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicDefense(5, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DealDamage("Patroclus", 6)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName("Achilles")
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                .Build())
                .ConfigDeck(d => d
                    .AddBasicAttack(5, amount: 10)
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
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertFighter("Achilles")
            .IsAtFullHealth();
    }
}
