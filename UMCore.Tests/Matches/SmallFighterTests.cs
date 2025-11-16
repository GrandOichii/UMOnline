namespace UMCore.Tests.Matches;

public class SmallFighterTests
{
    [Fact]
    public async Task InitialPlacementCheck()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0], spawnNumber: 2) // Foo2
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
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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
        
        match.AssertNode(0)
            .DoesntHaveFighterWithName("Bar");
        match.AssertNode(1)
            .DoesntHaveFighterWithName("Bar");
        match.AssertNode(2)
            .DoesntHaveFighterWithName("Bar");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task FighterPlacement(int nodeId)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0], spawnNumber: 2) // Foo2
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", nodeId)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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
        
        match.AssertNode(0)
            .HasFighterWithName("Foo1");
        match.AssertNode(nodeId)
            .HasFighterWithName("Bar");
        match.AssertNode(2)
            .HasFighterWithName("Foo2");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public async Task CantAddMore(int nodeId)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0], spawnNumber: 2) // Foo2
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", nodeId)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", nodeId)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", nodeId)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", nodeId)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", nodeId)
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
                    .Build()
                )
                .Build()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedUnintentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task FriendlyMoveThrough()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // Bar
            .AddNode(2, [0])                 // 
            .AddNode(3, [0], spawnNumber: 2) // Foo2
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 1)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Foo1")
                    .WithName("Bar")
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(2)
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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

        match.AssertNode(1)
            .HasFighterWithName("Bar");
        match.AssertNode(2)
            .HasFighterWithName("Foo1");
        match.AssertNode(3)
            .HasFighterWithName("Foo2");
    }

    [Fact]
    public async Task SmallEndMovementOnOccupied()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // Bar
            .AddNode(2, [0], spawnNumber: 2) // Foo2
            .AddNode(3, [0])                 // 
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 1)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Foo1")
                    .WithName("Bar")
                )
                .ConfigPathChoices(c => c
                    .First()
                    .FirstStopsAtId(2)
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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

        match.AssertNode(0)
            .HasFighterWithName("Foo1");
        match.AssertNode(2)
            .HasFighterWithName("Foo2");
        match.AssertNode(2)
            .HasFighterWithName("Bar");
    }

    [Fact]
    public async Task SmallMoveThroughOpposing()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // Bar
            .AddNode(2, [0], spawnNumber: 2) // Foo2
            .AddNode(3, [0])                 // 
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 1)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Foo1")
                    .WithName("Bar")
                )
                .ConfigPathChoices(c => c
                    .First()
                    .FirstStopsAtId(3)
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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

        match.AssertNode(0)
            .HasFighterWithName("Foo1");
        match.AssertNode(3)
            .HasFighterWithName("Bar");
        match.AssertNode(2)
            .HasFighterWithName("Foo2");
    }

    [Fact]
    public async Task OpposingMoveThrough()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .FirstPlayer(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0])                 // Bar
            .AddNode(3, [0], spawnNumber: 2) // Foo2
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                // .ConfigActions(a => a
                //     .DeclareWinner()
                //     .CrashMatch()
                // )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 2)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(1)
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertNode(1)
            .HasFighterWithName("Foo2");
        match.AssertNode(2)
            .HasFighterWithName("Bar");
    }

    [Fact]
    public async Task CantMoveToFull() {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // Bar x 4
            .AddNode(2, [0])                 // Bar
            .AddNode(3, [0], spawnNumber: 2) // Foo2
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 2)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .InNodeWithId(2)
                    .NTimes(5, nc => nc.First())
                )
                .ConfigPathChoices(c => c
                    .Assert(a => a
                        .CantStopAtNodeWithId(1)
                    )
                    .NTimes(6, nc => nc.First())
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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

        match.AssertNode(0)
            .FightersCount(1)
            .HasFighterWithName("Foo1");
        match.AssertNode(1)
            .FightersCount(4)
            .HasFighterWithName("Bar");
        match.AssertNode(2)
            .FightersCount(1)
            .HasFighterWithName("Bar");
        match.AssertNode(3)
            .FightersCount(1)
            .HasFighterWithName("Foo2");
    }

    [Fact]
    public async Task CanMoveToFullOpposingSmall() {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // Bar2 x 3
            .AddNode(2, [0])                 // Bar1
            .AddNode(3, [0], spawnNumber: 2) // Foo2
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar2", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar2", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar2", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar1", 2)
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigFighterChoices(c => c
                    .InNodeWithId(2)
                    .First()
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(1)
                    .First()
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar1", "Bar1")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar2", "Bar2")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
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

        match.AssertNode(0)
            .FightersCount(1)
            .HasFighterWithName("Foo1");
        match.AssertNode(1)
            .FightersCount(4)
            .HasFighterWithName("Bar1")
            .HasFighterWithName("Bar2");
        match.AssertNode(2)
            .FightersCount(0);
        match.AssertNode(3)
            .FightersCount(1)
            .HasFighterWithName("Foo2");
    }

    [Fact]
    public async Task SmallAttackOpposingAdjacent() {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0])                 // Bar
            .AddNode(3, [0], spawnNumber: 2) // Foo2
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 2)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Bar")
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(1)
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
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(0)
            .HasFighterWithName("Foo1");
        match.AssertNode(2)
            .HasFighterWithName("Bar");
        match.AssertNode(3)
            .HasFighterWithName("Foo2");

        match.AssertFighter("Foo2")
            .HasDamage(1);
    }

    [Fact]
    public async Task SmallAttackOpposingOnSameNode() {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0])                 // 
            .AddNode(3, [0], spawnNumber: 2) // Foo2 + Bar
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 3)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstByFighterWithName("Bar")
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Build()
                )
                .ConfigDeck(d => d
                    .AddBasicVersatile(1)
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
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(0)
            .HasFighterWithName("Foo1");
        match.AssertNode(3)
            .HasFighterWithName("Bar")
            .HasFighterWithName("Foo2");

        match.AssertFighter("Foo2")
            .HasDamage(1);
    }

    [Fact]
    public async Task OpposingAttackSmallAdjacent() {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0])                 // Bar
            .AddNode(3, [0], spawnNumber: 1) // Foo2
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigCardChoices(c => c.Nothing())
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Health(10)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 2)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName("Bar")
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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
            .HasCardsInHand(0)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertNode(0)
            .HasFighterWithName("Foo1");
        match.AssertNode(2)
            .HasFighterWithName("Bar");
        match.AssertNode(3)
            .HasFighterWithName("Foo2");

        match.AssertFighterInNode(2, "Bar")
            .HasDamage(1);
    }

    [Fact]
    public async Task OpposingAttackSmallInSameNode() {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .FirstPlayer(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Foo1
            .AddNode(1, [0])                 // 
            .AddNode(2, [0])                 // Bar
            .AddNode(3, [0], spawnNumber: 1) // Foo2
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigCardChoices(c => c.Nothing())
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar", "Bar")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Health(10)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar", 3)
                    .Attack()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigAttackChoices(c => c
                    .FirstTargetingFighterWithName("Bar")
                )
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
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
            .HasCardsInHand(0)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();

        match.AssertNode(0)
            .HasFighterWithName("Foo1");
        match.AssertNode(3)
            .HasFighterWithName("Bar")
            .HasFighterWithName("Foo2");

        match.AssertFighterInNode(3, "Bar")
            .HasDamage(1);
    }

    // If a small fighter takes damage, all small fighters of the same type in that space take an equal amount of damage. (So, if a squirrel takes any damage, it and all other squirrels in its space are defeated.)
    [Fact]
    public async Task DamageCheck() {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Foo1
            .AddNode(1, [0])                 // Bar1
            .AddNode(2, [0])                 // 2 x Bar1 + Bar2
            .AddNode(3, [0], spawnNumber: 2) // Foo2
            .ConnectAll()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar1", 1)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar1", 2)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar1", 2)
                    .PlaceOffboardFighterWithNameInNodeWithId("Bar2", 2)
                    .DealDamage(2, "Bar1", 1)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .Build(),
            new LoadoutTemplateBuilder("Main")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar1", "Bar1")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Health(10)
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .Build(),
            new LoadoutTemplateBuilder("Foo")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
                    .Build()
                )
                .AddFighter(new FighterTemplateBuilder("Bar2", "Bar2")
                    .IsSmall()
                    .IsSidekick()
                    .Amount(8)
                    .Health(10)
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
            .HasCardsInHand(0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(0)
            .FightersCount(1)
            .HasFighterWithName("Foo1");
        match.AssertNode(1)
            .FightersCount(1)
            .HasFighterWithName("Bar1");
        match.AssertNode(2)
            .FightersCount(3)
            .HasFighterWithName("Bar1")
            .HasFighterWithName("Bar2");
        match.AssertNode(3)
            .FightersCount(1)
            .HasFighterWithName("Foo2");

        match.AssertFighterInNode(0)
            .IsAtFullHealth();
        match.AssertFighterInNode(1)
            .IsAtFullHealth();
        match.AssertAllFightersInNode(2, "Bar1")
            .HaveDamage(1);
        match.AssertFighterInNode(3)
            .IsAtFullHealth();
    }
}