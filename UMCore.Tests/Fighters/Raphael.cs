using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class RaphaelTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Raphael")
        .Load("../../../../.generated/loadouts/Raphael/Raphael.json")
        .ClearDeck();

    [Theory]
    [InlineData("Raphael")]
    [InlineData("Casey Jones")]
    public async Task WonAttack(string character)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Raphael
            .AddNode(1, [0])                 // Casey Jones
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
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(character)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(1)
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
                    .AddBasicVersatile(0)
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
            .HasUnspentActions(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("Raphael")]
    [InlineData("Casey Jones")]
    public async Task LostAttack(string character)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Raphael
            .AddNode(1, [0])                 // Casey Jones
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
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName(character)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(1)
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
                    .AddBasicVersatile(2)
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
            .HasUnspentActions(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData("Raphael")]
    [InlineData("Casey Jones")]
    public async Task LostDefense(string character)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Raphael
            .AddNode(1, [0])                 // Casey Jones
            .AddNode(2, [0], spawnNumber: 2) // Foo
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(0)
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
                    .FirstTargetingFighterWithName(character)
                )
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
            .HasNoUnspentActions()
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

}
