using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class EredinTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Eredin")
        .Load("../../../../.generated/loadouts/Eredin/Eredin.json")
        .ClearDeck();

    [Fact]
    public async Task BaselineAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(3)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0]) 
            .AddNode(1, [0]) 
            .AddNode(2, [0]) 
            .AddNode(3, [0], spawnNumber: 1) // Eredin
            .AddNode(4, [0], spawnNumber: 2) // Foo
            .AddNode(5, [0])                 // Red Rider 1
            .AddNode(6, [0])                 // Red Rider 2
            .AddNode(7, [0])                 // Red Rider 3
            .AddNode(8, [0])                 // Red Rider 4
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
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(5)
                    .WithId(6)
                    .WithId(7)
                    .WithId(8)
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .CantReachNodeWithId(0)
                        .CanReachNodeWithId(1)
                        .CanReachNodeWithId(2)
                    )
                    .NTimes(5, nc => nc.First())
                    // .First()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Eredin")
                    .NTimes(4, nc => nc.First())
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Eredin")
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .IsAtFullHealth();
    }

    [Fact]
    public async Task BaselineDefense()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0]) 
            .AddNode(1, [0]) 
            .AddNode(2, [0]) 
            .AddNode(3, [0], spawnNumber: 1) // Eredin
            .AddNode(4, [0], spawnNumber: 2) // Foo
            .AddNode(5, [0])                 // Red Rider 1
            .AddNode(6, [0])                 // Red Rider 2
            .AddNode(7, [0])                 // Red Rider 3
            .AddNode(8, [0])                 // Red Rider 4
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
                    .WithId(5)
                    .WithId(6)
                    .WithId(7)
                    .WithId(8)
                )
                .ConfigHandCardChoices(c => c.First())
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(0)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName("Eredin")
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Eredin")
            .HasDamage(1);
    }

    [Fact]
    public async Task EnragedAttack()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(3)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0])
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0], spawnNumber: 1) // Eredin
            .AddNode(4, [0], spawnNumber: 2) // Foo
            .AddNode(5, [0])                 // Red Rider 1
            .AddNode(6, [0])                 // Red Rider 2
            .AddNode(7, [0])                 // Red Rider 3
            .AddNode(8, [0])                 // Red Rider 4
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DealDamageToAllFightersWithName("Red Rider", 1)
                    .Attack()
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(5)
                    .WithId(6)
                    .WithId(7)
                    .WithId(8)
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .CanReachNodeWithId(0)
                        .CanReachNodeWithId(1)
                        .CanReachNodeWithId(2)
                    )
                    .First()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Eredin")
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Eredin")
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Foo")
            .HasDamage(1);
    }
    
    [Fact]
    public async Task EnragedDefense()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0]) 
            .AddNode(1, [0]) 
            .AddNode(2, [0]) 
            .AddNode(3, [0], spawnNumber: 1) // Eredin
            .AddNode(4, [0], spawnNumber: 2) // Foo
            .AddNode(5, [0])                 // Red Rider 1
            .AddNode(6, [0])                 // Red Rider 2
            .AddNode(7, [0])                 // Red Rider 3
            .AddNode(8, [0])                 // Red Rider 4
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
                    .WithId(5)
                    .WithId(6)
                    .WithId(7)
                    .WithId(8)
                )
                .ConfigHandCardChoices(c => c.First())
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicVersatile(0)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DealDamageToAllFightersWithName("Red Rider", 1)
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName("Eredin")
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter("Eredin")
            .IsAtFullHealth();
    }
}
