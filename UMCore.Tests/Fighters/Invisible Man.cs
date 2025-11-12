
namespace UMCore.Tests.Fighters;

public class InvisibleManTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Invisible Man")
        .Load("../../../../.generated/loadouts/Invisible Man/Invisible Man.json")
        .ClearDeck();

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(2, 3, 4)]
    [InlineData(1, 3, 4)]
    public async Task ValidTokenPlacement(int token1NodeId, int token2NodeId, int token3NodeId)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            // .FirstPlayer(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // invisible man
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0])
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
                    .WithId(token1NodeId)
                    .WithId(token2NodeId)
                    .WithId(token3NodeId)
                )
                .Build(),
            GetLoadoutBuilder().Build()
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

        match.AssertNode(token1NodeId)
            .HasToken("Fog");
        match.AssertNode(token2NodeId)
            .HasToken("Fog");
        match.AssertNode(token3NodeId)
            .HasToken("Fog");
    }

    [Fact]
    public async Task InvalidTokenPlacement()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            // .FirstPlayer(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // invisible man
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [1])
            .AddNode(5, [0])
            .AddNode(6, [0], spawnNumber: 2) // foo
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
                    .AssertOptionsEquivalent([1, 2, 3, 5])
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                )
                .Build(),
            GetLoadoutBuilder().Build()
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

        match.AssertNode(1)
            .HasToken("Fog");
        match.AssertNode(2)
            .HasToken("Fog");
        match.AssertNode(3)
            .HasToken("Fog");
    }

    [Fact]
    public async Task BaselineDefenseCheck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(5, [0])
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(0, [0], spawnNumber: 2) // invisible man
            .AddNode(4, [0], spawnNumber: 1) // foo
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
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(3)
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicDefense(3, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicAttack(5)
                )
                .Build()
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

        match.AssertNode(3)
            .HasToken("Fog");
        match.AssertNode(1)
            .HasToken("Fog");
        match.AssertNode(2)
            .HasToken("Fog");

        match.AssertFighter("Invisible Man")
            .HasDamage(2);
    }

    [Fact]
    public async Task DefenseOnFogCheck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(5, [0])
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(0, [0], spawnNumber: 2) // invisible man
            .AddNode(4, [0], spawnNumber: 1) // foo
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
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(3)
                    .WithId(1)
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .AddBasicDefense(3, amount: 10)
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .MoveAllTokens(3, 0)
                    .Attack()
                )
                .ConfigAttackChoices(c => c
                    .First()
                )
            .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo", "Foo")
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicAttack(5)
                )
                .Build()
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

        match.AssertNode(0)
            .HasToken("Fog");
        match.AssertNode(1)
            .HasToken("Fog");
        match.AssertNode(2)
            .HasToken("Fog");

        match.AssertFighter("Invisible Man")
            .HasDamage(1);
    }
    
    [Fact]
    public async Task FogTraversalCheck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0]) // fog token 1
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0])
            .AddNode(5, [0])
            .AddNode(6, [0], spawnNumber: 1) // invisible man + fog token 2
            .AddNode(7, [0])
            .AddNode(8, [0])
            .AddNode(9, [0])
            .AddNode(10, [0])
            .AddNode(11, [0])
            .AddNode(12, [0]) // fog token 3
            .AddNode(13, [0], spawnNumber: 2) // foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .MoveAllTokens(5, 6)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .CanReachNodeWithId(0)
                        .CanReachNodeWithId(1)
                        .CantReachNodeWithId(2)
                        .CantReachNodeWithId(3)
                        .CanReachNodeWithId(4)
                        .CanReachNodeWithId(5)
                        .CanReachNodeWithId(6)
                        .CanReachNodeWithId(7)
                        .CanReachNodeWithId(8)
                        .CantReachNodeWithId(9)
                        .CantReachNodeWithId(10)
                        .CanReachNodeWithId(11)
                        .CanReachNodeWithId(12)
                    )
                )
                .ConfigPathChoices(c => c
                    .First()
                )
                .ConfigNodeChoices(c => c
                    .WithId(0)
                    .WithId(5)
                    .WithId(12)
                )
                .Build(),
            GetLoadoutBuilder().Build()
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
    }

}