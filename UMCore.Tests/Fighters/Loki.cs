using System.ComponentModel;
using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class LokiTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Loki")
        .Load("../../../../.generated/loadouts/Loki/Loki.json")
        .ClearDeck();

    [Fact]
    public async Task Main_TrickScheme()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Loki
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
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigCardChoices(c => c.First())
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Scheme()
                        .HasLabel("trick")
                        .Build()
                    )
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
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(1)
            .IsNotWinner();
    }

    [Fact]
    public async Task Main_TrickAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Loki
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
                .ConfigAttackChoices(c => c.First())
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(1)
                        .Versatile()
                        .HasLabel("trick")
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigCardChoices(c => c
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
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(1)
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(1);
    }

    [Fact]
    public async Task Main_TrickDefend()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Loki
            .AddNode(1, [0], spawnNumber: 1) // Foo
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
                .ConfigCardChoices(c => c
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(0)
                        .Versatile()
                        .HasLabel("trick")
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                )
                .ConfigAttackChoices(c => c.First())
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(1)
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
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(0)
            .HasCardsInHand(1)
            .IsNotWinner();
    }

    [Fact]
    public async Task Opp_TrickScheme()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Loki
            .AddNode(1, [0], spawnNumber: 1) // Foo
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
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Scheme()
                )
                .ConfigCardChoices(c => c.First())
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Scheme()
                        .HasLabel("trick")
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
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsNotWinner();
    }

    [Fact]
    public async Task Opp_TrickAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Loki
            .AddNode(1, [0], spawnNumber: 1) // Foo
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
                .ConfigCardChoices(c => c
                    .Nothing()
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                )
                .ConfigAttackChoices(c => c.First())
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(1)
                        .Versatile()
                        .HasLabel("trick")
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
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsNotWinner();
    }

    [Fact]
    public async Task Opp_TrickDefend()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Loki
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
                .ConfigAttackChoices(c => c.First())
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(1)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigCardChoices(c => c
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(1)
                        .Versatile()
                        .HasLabel("trick")
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
            .HasCardsInDiscardPile(2)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsNotWinner();
    }

    [Theory]
    [InlineData("Hand", 1, 0)]
    [InlineData("Top of deck", 0, 1)]
    public async Task Opp_DiscardTrick(string targetChoice, int expectedMainHandCount, int expectedMainDeckCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .MaxHandSize(0)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Loki
            .AddNode(1, [0], spawnNumber: 1) // Foo
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
                    .Choose(targetChoice)
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .ConfigCardChoices(c => c.First())
                .ConfigFighterChoices(c => c.First())
                .ConfigPathChoices(c => c.First())
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(1)
                        .Versatile()
                        .HasLabel("trick")
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
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(expectedMainDeckCount)
            .HasCardsInHand(expectedMainHandCount)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsNotWinner();
    }

    [Fact]
    public async Task Main_DiscardTrick()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(1)
            .MaxHandSize(0)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Loki
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
                    .Manoeuvre()
                )
                .ConfigCardChoices(c => c.Nothing())
                .ConfigFighterChoices(c => c.First())
                .ConfigPathChoices(c => c.First())
                .ConfigCardChoices(c => c.First())
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Scheme()
                        .HasLabel("trick")
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
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
            .HasCardsInDiscardPile(1)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
    }

    [Fact]
    public async Task LokiMovement()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(3)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Loki
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0])
            .AddNode(5, [0])
            .AddNode(6, [0])
            .AddNode(7, [0])
            .AddNode(8, [0])
            .AddNode(9, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
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
                .ConfigFighterChoices(c => c.First())
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .CanStopAtNodeWithId(0)
                        .CanStopAtNodeWithId(1)
                        .CanStopAtNodeWithId(2)
                        .CanStopAtNodeWithId(3)
                        .CanStopAtNodeWithId(4)
                        .CantStopAtNodeWithId(5)
                        .CantStopAtNodeWithId(7)
                        .CantStopAtNodeWithId(8)
                        .CantStopAtNodeWithId(9)
                    )
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Value(1)
                        .Amount(2)
                        .Versatile()
                        .HasLabel("trick")
                        .Build()
                    )
                    .AddBasicAttack(1)
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
            .HasCardsInDeck(0)
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .HasCardsInDiscardPile(0)
            .HasCardsInDeck(0)
            .HasCardsInHand(3)
            .IsNotWinner();
    }
}
