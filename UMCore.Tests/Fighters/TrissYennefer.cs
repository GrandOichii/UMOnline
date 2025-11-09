using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class TrisYenneferTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Triss & Yennefer")
        .Load("../../../../.generated/loadouts/Triss & Yennefer/Triss & Yennefer.json")
        .ClearDeck();

    [Fact]
    public async Task YenneferAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Yennefer
            .AddNode(1, [0])                 // Triss
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
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Yennefer")
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(0, boost: 1, amount: 2)
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .IsAtFullHealth();
    }

    [Fact]
    public async Task TrissScheme()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Yennefer
            .AddNode(1, [0])                 // Triss
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
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigFighterChoices(c => c
                    .WithName("Triss")
                    .WithName("Foo")
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(2);
    }

    [Fact]
    public async Task YenneferScheme()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(2)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Yennefer
            .AddNode(1, [0])                 // Triss
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
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigFighterChoices(c => c
                    .WithName("Yennefer")
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .IsAtFullHealth();
    }
}
