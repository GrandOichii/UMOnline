using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class MichelangeloTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Michelangelo")
        .Load("../../../../.generated/loadouts/Michelangelo/Michelangelo.json")
        .ClearDeck();

    [Fact]
    public async Task CheckHandSize()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Michelangelo
            .AddNode(1, [0])                 // April O'Neil
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
                    .AddBasicVersatile(0, amount: 10)
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
            .HasCardsInHand(3)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("Michelangelo")]
    [InlineData("April O'Neil")]
    public async Task AfterScheme(string character)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Michelangelo
            .AddNode(1, [0])                 // April O'Neil
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
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigHandCardChoices(c => c.First())
                .ConfigFighterChoices(c => c.WithName(character))
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
            .HasCardsInHand(3)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("Michelangelo")]
    [InlineData("April O'Neil")]
    public async Task AfterAttack(string character)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .ActionsPerTurn(2)
            .InitialHandSize(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Michelangelo
            .AddNode(1, [0])                 // April O'Neil
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
                    .FirstByFighterWithName(character)
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
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
                .ConfigHandCardChoices(c => c.Nothing())
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
            .HasCardsInHand(3)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }
}
