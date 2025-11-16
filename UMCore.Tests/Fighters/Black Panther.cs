using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class BlackPantherTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Black Panther")
        .Load("../../../../.generated/loadouts/Black Panther/Black Panther.json")
        .ClearDeck();

    
    [Fact]
    public async Task HasVibraniumSuit()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .ManoeuvreDrawAmount(0)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Black Panther
            .AddNode(1, [0])                 // Shuri
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
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 2, boost: 1)
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
            .HasCardZone("VIBRANIUM SUIT")
            .HasCardsInZone("VIBRANIUM SUIT", 0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .DoesntHaveCardZone("VIBRANIUM SUIT")
            .IsNotWinner();
    }

    [Theory]
    [InlineData("HAND", "Black Panther", 1, 1)]
    [InlineData("HAND", "Shuri", 1, 1)]
    [InlineData("VIBRANIUM SUIT", "Black Panther", 2, 0)]
    [InlineData("VIBRANIUM SUIT", "Shuri", 2, 0)]
    public async Task AttackBoost_Accept(string boostSource, string attacker, int expectedHandCount, int expectedSuitCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(2)
            .ManoeuvreDrawAmount(0)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Black Panther
            .AddNode(1, [0])                 // Shuri
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .MoveTopCards(0, 1, "DECK", "VIBRANIUM SUIT")
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(attacker)
                )
                .ConfigCardChoices(c => c.First())
                .ConfigStringChoices(c => c.Choose(boostSource))
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Versatile()
                        .Boost(1)
                        .Value(1)
                        .Amount(4)
                        .Script("""
                        :DuringCombat(
                            'During combat: you may BOOST this card.',
                            UM.Effects:AllowBoost(UM.Number:Static(1), true)
                        )
                        """)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigCardChoices(c => c.Nothing())
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
            .HasCardsInHand(expectedHandCount)
            .HasCardsInZone("VIBRANIUM SUIT", expectedSuitCount)
            .HasCardsInDeck(0)
            .HasCardsInDiscardPile(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("HAND", "Black Panther", 1, 1)]
    [InlineData("HAND", "Shuri", 1, 1)]
    [InlineData("VIBRANIUM SUIT", "Black Panther", 1, 1)]
    [InlineData("VIBRANIUM SUIT", "Shuri", 1, 1)]
    public async Task AttackBoost_Decline(string boostSource, string attacker, int expectedHandCount, int expectedSuitCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(2)
            .ManoeuvreDrawAmount(0)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Black Panther
            .AddNode(1, [0])                 // Shuri
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .MoveTopCards(0, 1, "DECK", "VIBRANIUM SUIT")
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(attacker)
                )
                .ConfigCardChoices(c => c.Nothing())
                .ConfigStringChoices(c => c.Choose(boostSource))
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new CardTemplateBuilder()
                        .Versatile()
                        .Boost(1)
                        .Value(1)
                        .Amount(4)
                        .Script("""
                        :DuringCombat(
                            'During combat: you may BOOST this card.',
                            UM.Effects:AllowBoost(UM.Number:Static(1), true)
                        )
                        """)
                        .Build()
                    )
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigCardChoices(c => c.Nothing())
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
            .HasCardsInHand(expectedHandCount)
            .HasCardsInZone("VIBRANIUM SUIT", expectedSuitCount)
            .HasCardsInDeck(1)
            .HasCardsInDiscardPile(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("VIBRANIUM SUIT", 2, 0)]
    [InlineData("HAND", 1, 1)]
    public async Task ManoeuvreBoostSources_AcceptBoost(string boostSource, int expectedHandCount, int expectedSuitCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .ManoeuvreDrawAmount(0)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Black Panther
            .AddNode(1, [0])                 // Shuri
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
                    .MoveTopCards(0, 1, "DECK", "VIBRANIUM SUIT")
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Assert(a => a
                        .EquivalentTo(["HAND", "VIBRANIUM SUIT"])
                    )
                    .Choose(boostSource)
                )
                .ConfigCardChoices(c => c.First())
                .ConfigFighterChoices(c => c.First().First())
                .ConfigPathChoices(c => c.First().First())
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 3, boost: 1)
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
            .HasCardsInDeck(0)
            .HasCardsInDiscardPile(1)
            .HasCardsInHand(expectedHandCount)
            .HasCardsInZone("VIBRANIUM SUIT", expectedSuitCount)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("VIBRANIUM SUIT", 1, 1)]
    [InlineData("HAND", 1, 1)]
    public async Task ManoeuvreBoostSources_DeclineBoost(string boostSource, int expectedHandCount, int expectedSuitCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .ManoeuvreDrawAmount(0)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Black Panther
            .AddNode(1, [0])                 // Shuri
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
                    .MoveTopCards(0, 1, "DECK", "VIBRANIUM SUIT")
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Assert(a => a
                        .EquivalentTo(["HAND", "VIBRANIUM SUIT"])
                    )
                    .Choose(boostSource)
                )
                .ConfigCardChoices(c => c.Nothing())
                .ConfigFighterChoices(c => c.First().First())
                .ConfigPathChoices(c => c.First().First())
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme(amount: 3, boost: 1)
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
            .HasCardsInDeck(1)
            .HasCardsInDiscardPile(0)
            .HasCardsInHand(expectedHandCount)
            .HasCardsInZone("VIBRANIUM SUIT", expectedSuitCount)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}
