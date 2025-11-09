using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class Philippa
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Philippa")
        .Load("../../../../.generated/loadouts/Philippa/Philippa.json")
        .ClearDeck();

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public async Task CantTrigger(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Philippa
            .AddNode(1, [0])                 // Dijkstra
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
                    .Scheme()
                )
                .ConfigHandCardChoices(c => c.First())
                .ConfigFighterChoices(c => c.First())
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
            .HasCardsInHand(handSize - 1)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task DeclineTrigger(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Philippa
            .AddNode(1, [0])                 // Dijkstra
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
                    .Scheme()
                )
                .ConfigHandCardChoices(c => c.First())
                .ConfigFighterChoices(c => c.First())
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .No()
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
            .HasCardsInHand(handSize - 1)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(3)]
    [InlineData(2)]
    [InlineData(1)]
    public async Task AcceptTrigger(int handSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(handSize)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Philippa
            .AddNode(1, [0])                 // Dijkstra
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
                    .Scheme()
                )
                .ConfigHandCardChoices(c => c.First())
                .ConfigFighterChoices(c => c.First())
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Yes()
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
            .HasCardsInHand(4)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

    [Fact]
    public async Task Lt4CardsInDeck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(1)
            .ExhaustDamage(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Philippa
            .AddNode(1, [0])                 // Dijkstra
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
                    .Scheme()
                )
                .ConfigHandCardChoices(c => c.First())
                .ConfigFighterChoices(c => c.First())
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .Yes()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicScheme()
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
            .DidntThrow();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }
}
