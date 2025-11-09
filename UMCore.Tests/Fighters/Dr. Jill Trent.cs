using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class DrJillTrentTests
{
    private static readonly string ATTR_KEY = "GADGET";

    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Dr. Jill Trent")
        .Load("../../../../.generated/loadouts/Dr. Jill Trent/Dr. Jill Trent.json")
        .ClearDeck();

    [Theory]
    [InlineData("Hypnoray Blaster", "blaster")]
    [InlineData("Ultrabiotic Tonic", "tonic")]
    public async Task CheckChosenGadget(string chosenGadget, string expectedAttr)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Dr. Jill Trent
            .AddNode(1, [0])                 // Daisy
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Choose(chosenGadget)
                )
                .Build(),
            GetLoadoutBuilder().Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .StringAttrEq(ATTR_KEY, expectedAttr)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("Daisy", 1, 0, 1, 1)]
    [InlineData("Daisy", 0, 1, 1, 1)]
    [InlineData("Dr. Jill Trent", 1, 0, 0, 1)]
    [InlineData("Dr. Jill Trent", 0, 1, 1, 1)]
    public async Task CheckTonicAttack(
        string attacker,
        int attackerValue,
        int defenderValue,
        int expectedJillDamage,
        int expectedDaisyDamage)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Dr. Jill Trent
            .AddNode(1, [0])                 // Daisy
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DealDamage("Dr. Jill Trent", 1)
                    .DealDamage("Daisy", 1)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(attacker)
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Choose("Ultrabiotic Tonic")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(attackerValue)
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
                    .AddBasicVersatile(defenderValue)
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

        match.AssertFighter("Dr. Jill Trent")
            .HasDamage(expectedJillDamage);
        match.AssertFighter("Daisy")
            .HasDamage(expectedDaisyDamage);
    }

    [Theory]
    [InlineData("Daisy", 1, 0, 1, 2)]
    [InlineData("Daisy", 0, 1, 1, 1)]
    [InlineData("Dr. Jill Trent", 1, 0, 2, 1)]
    [InlineData("Dr. Jill Trent", 0, 1, 1, 1)]
    public async Task CheckTonicDefense(
        string defender,
        int attackerValue,
        int defenderValue,
        int expectedJillDamage,
        int expectedDaisyDamage)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Dr. Jill Trent
            .AddNode(1, [0])                 // Daisy
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c.First())
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(defenderValue)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .SetStrAttr(0, "gadget", "tonic")
                    .DealDamage("Dr. Jill Trent", 1)
                    .DealDamage("Daisy", 1)
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName(defender)
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(attackerValue)
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

        match.AssertFighter("Dr. Jill Trent")
            .HasDamage(expectedJillDamage);
        match.AssertFighter("Daisy")
            .HasDamage(expectedDaisyDamage);
    }

    [Theory]
    [InlineData("Daisy", 1, 0, 1)]
    [InlineData("Daisy", 0, 1, 0)]
    [InlineData("Dr. Jill Trent", 1, 0, 1)]
    [InlineData("Dr. Jill Trent", 0, 1, 1)]
    public async Task CheckBlasterAttack(
        string attacker,
        int attackerValue,
        int defenderValue,
        int expectedFooDamage)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Dr. Jill Trent
            .AddNode(1, [0])                 // Daisy
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DealDamage("Dr. Jill Trent", 1)
                    .DealDamage("Daisy", 1)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(attacker)
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Choose("Hypnoray Blaster")
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(attackerValue, boost: 2, amount: 2)
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
                    .AddBasicVersatile(defenderValue)
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
            .HasDamage(expectedFooDamage);
    }

    [Theory]
    [InlineData("Daisy", 1, 0, 1, 2)]
    [InlineData("Daisy", 0, 1, 1, 1)]
    [InlineData("Dr. Jill Trent", 1, 0, 2, 1)]
    [InlineData("Dr. Jill Trent", 0, 1, 1, 1)]
    public async Task CheckBlasterDefense(
        string defender,
        int attackerValue,
        int defenderValue,
        int expectedJillDamage,
        int expectedDaisyDamage)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Dr. Jill Trent
            .AddNode(1, [0])                 // Daisy
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c.First())
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(defenderValue)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .SetStrAttr(0, "gadget", "blaster")
                    .DealDamage("Dr. Jill Trent", 1)
                    .DealDamage("Daisy", 1)
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName(defender)
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(attackerValue)
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

        match.AssertFighter("Dr. Jill Trent")
            .HasDamage(expectedJillDamage);
        match.AssertFighter("Daisy")
            .HasDamage(expectedDaisyDamage);
    }

}
