using System.Text.Json;
using Shouldly;

namespace UMCore.Tests.Fighters;

public class ElektraTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Elektra")
        .Load("../../../../.generated/loadouts/Elektra/Elektra.json")
        .ClearDeck();

    [Fact]
    public async Task CheckInitialState()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Elektra
            .AddNode(1, [0])                 // The Hand 1
            .AddNode(2, [0])                 // The Hand 2
            .AddNode(3, [0])                 // The Hand 3
            .AddNode(4, [0])                 // The Hand 4
            .AddNode(5, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5)
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
                // .ConfigDeck(d => d
                //     .AddBasicVersatile(3)
                // )
                .Build()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .StringAttrEq("RESURRECTED", "N")
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task Resurrection1()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(1)
            .ExhaustDamage(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Elektra
            .AddNode(1, [1])                 // The Hand 1
            .AddNode(2, [2])                 // The Hand 2
            .AddNode(3, [3])                 // The Hand 3
            .AddNode(4, [4])                 // The Hand 4
            .AddNode(5, [5])
            .AddNode(6, [6], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(10)
                    .DealDamage("Elektra", 6)
                    .Manoeuvre()
                    // turn should end after the manoeuvre
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                    // after resurrection
                    .AssertOptionsEquivalent([0, 1, 2, 3, 4, 5])
                    .WithId(0) // Elektra
                    .AssertOptionsEquivalent([1, 2, 3, 4, 5])
                    .WithId(1) // The Hand 1
                    .AssertOptionsEquivalent([2, 3, 4, 5])
                    .WithId(2) // The Hand 2
                    .AssertOptionsEquivalent([3, 4, 5])
                    .WithId(3) // The Hand 3
                    .AssertOptionsEquivalent([4, 5])
                    .WithId(4) // The Hand 4
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5, amount: 11)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(3, amount: 10)
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
            .StringAttrEq("RESURRECTED", "Y")
            .HasFighterWithName("Elektra Resurrected")
            .HasCardsInDeck(10)
            .HasCardsInHand(1)
            .HasCardsInDiscardPile(0)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Elektra")
            .HasName("Elektra Resurrected")
            .HasHealth(9)
            .HasMaxHealth(9);

        match.AssertNode(0)
            .HasFighterWithName("Elektra Resurrected");
        match.AssertNode(1)
            .HasFighterWithName("The Hand");
        match.AssertNode(2)
            .HasFighterWithName("The Hand");
        match.AssertNode(3)
            .HasFighterWithName("The Hand");
        match.AssertNode(4)
            .HasFighterWithName("The Hand");
    }

    [Fact]
    public async Task Resurrection2()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .ExhaustDamage(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Elektra
            .AddNode(1, [1])                 // The Hand 1
            .AddNode(2, [2])                 // The Hand 2
            .AddNode(3, [3])                 // The Hand 3
            .AddNode(4, [4])                 // The Hand 4
            .AddNode(5, [5])
            .AddNode(6, [6], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(10)
                    .DealDamage("Elektra", 5)
                    .Manoeuvre()
                    .Manoeuvre()
                    // turn should end after the manoeuvre
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c.Nothing())
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                    // after resurrection
                    .AssertOptionsEquivalent([0, 1, 2, 3, 4, 5])
                    .WithId(0) // Elektra
                    .AssertOptionsEquivalent([1, 2, 3, 4, 5])
                    .WithId(1) // The Hand 1
                    .AssertOptionsEquivalent([2, 3, 4, 5])
                    .WithId(2) // The Hand 2
                    .AssertOptionsEquivalent([3, 4, 5])
                    .WithId(3) // The Hand 3
                    .AssertOptionsEquivalent([4, 5])
                    .WithId(4) // The Hand 4
                )
                .ConfigFighterChoices(c => c.First())
                .ConfigPathChoices(c => c.First())
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5, amount: 11)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                    .Manoeuvre()
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First()
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(3, amount: 10)
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
            .StringAttrEq("RESURRECTED", "Y")
            .HasFighterWithName("Elektra Resurrected")
            .HasCardsInDeck(10)
            .HasCardsInHand(1)
            .HasCardsInDiscardPile(0)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Elektra")
            .HasName("Elektra Resurrected")
            .HasHealth(9)
            .HasMaxHealth(9);

        match.AssertNode(0)
            .HasFighterWithName("Elektra Resurrected");
        match.AssertNode(1)
            .HasFighterWithName("The Hand");
        match.AssertNode(2)
            .HasFighterWithName("The Hand");
        match.AssertNode(3)
            .HasFighterWithName("The Hand");
        match.AssertNode(4)
            .HasFighterWithName("The Hand");
    }

    [Fact]
    public async Task SecondDeath()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(1)
            .ExhaustDamage(9)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Elektra
            .AddNode(1, [1])                 // The Hand 1
            .AddNode(2, [2])                 // The Hand 2
            .AddNode(3, [3])                 // The Hand 3
            .AddNode(4, [4])                 // The Hand 4
            .AddNode(5, [5])
            .AddNode(6, [6], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Mill(10)
                    .Manoeuvre()
                    // turn should end after the manoeuvre
                    .Mill(10)
                    .Manoeuvre()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                    // after resurrection
                    .AssertOptionsEquivalent([0, 1, 2, 3, 4, 5])
                    .WithId(0) // Elektra
                    .AssertOptionsEquivalent([1, 2, 3, 4, 5])
                    .WithId(1) // The Hand 1
                    .AssertOptionsEquivalent([2, 3, 4, 5])
                    .WithId(2) // The Hand 2
                    .AssertOptionsEquivalent([3, 4, 5])
                    .WithId(3) // The Hand 3
                    .AssertOptionsEquivalent([4, 5])
                    .WithId(4) // The Hand 4
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(5, amount: 11)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(3, amount: 10)
                )
                .Build()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .DidntThrow();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }
}
