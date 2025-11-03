using System.Text.Json;
using Shouldly;

namespace UMCore.Tests.Fighters;

public class DraculaTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Dracula")
        .Load("../../../../.generated/loadouts/Dracula/Dracula.json")
        .ClearDeck();

    [Fact]
    public async Task NoTrigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // dracula
            .AddNode(1, [0])
            .AddNode(2, [0])                 // sister 1
            .AddNode(3, [0])                 // sister 2
            .AddNode(4, [0])                 // sister 3
            .AddNode(5, [0], spawnNumber: 2) // foo
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
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(1, amount: 10)
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
            .HasCardsInHand(0)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertAllFighters()
            .AreAtFullHealth();
    }

    [Fact]
    public async Task Decline_Sister_Trigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // dracula
            .AddNode(2, [0])                 // sister 1
            .AddNode(1, [0])
            .AddNode(3, [0])                 // sister 2
            .AddNode(4, [0])                 // sister 3
            .AddNode(5, [0], spawnNumber: 2) // foo
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
                .ConfigNodeChoices(c => c
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(1, amount: 10)
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
            .HasCardsInHand(0)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertAllFighters()
            .AreAtFullHealth();
    }

    [Fact]
    public async Task Decline_Opponent_Trigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // dracula
            .AddNode(5, [0], spawnNumber: 2) // foo
            .AddNode(2, [0])                 // sister 1
            .AddNode(3, [0])                 // sister 2
            .AddNode(4, [0])                 // sister 3
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
                .ConfigNodeChoices(c => c
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(1, amount: 10)
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
            .HasCardsInHand(0)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertAllFighters()
            .AreAtFullHealth();
    }

    [Fact]
    public async Task Accept_Sister_Trigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // dracula
            .AddNode(2, [0])                 // sister 1
            .AddNode(1, [0])
            .AddNode(3, [0])                 // sister 2
            .AddNode(4, [0])                 // sister 3
            .AddNode(5, [0], spawnNumber: 2) // foo
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
                .ConfigNodeChoices(c => c
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(1, amount: 10)
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
            .HasCardsInHand(1)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.Match.Fighters.Where(f => !f.IsAlive()).Count().ShouldBe(1);
    }

    [Fact]
    public async Task Accept_Opponent_Trigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // dracula
            .AddNode(5, [0], spawnNumber: 2) // foo
            .AddNode(2, [0])                 // sister 1
            .AddNode(3, [0])                 // sister 2
            .AddNode(4, [0])                 // sister 3
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
                .ConfigNodeChoices(c => c
                    .WithId(2)
                    .WithId(3)
                    .WithId(4)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicAttack(1, amount: 10)
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
            .HasCardsInHand(1)
            .SetupCalled()
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(1);
    }
}