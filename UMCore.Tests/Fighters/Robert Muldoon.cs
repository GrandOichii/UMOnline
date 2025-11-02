namespace UMCore.Tests.Fighters;

public class RobertMuldoon
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("InGen")
        .Load("../../../../.generated/loadouts/InGen/InGen.json")
        .ClearDeck();

    [Fact]
    public async Task TurnStart_PlaceTrap()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0])
            .AddNode(5, [0], spawnNumber: 2)
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
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4) // trap
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            LoadoutTemplateBuilder.Foo()
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

        match.AssertNode(4)
            .HasToken("Trap");

        match.AssertToken("Trap")
            .HasAmount(7);
    }

    [Fact]
    public async Task TurnStart_DeclinePlaceTrap()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [0])
            .AddNode(5, [0], spawnNumber: 2)
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
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            LoadoutTemplateBuilder.Foo()
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

        match.AssertToken("Trap")
            .HasAmount(8);
    }

    [Fact]
    public async Task TurnStart_CantPlaceTrap_NoUnoccupiedNodes()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(5, [0], spawnNumber: 2)
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
                    .WithId(2)
                    .WithId(3)
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            LoadoutTemplateBuilder.Foo()
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

        match.AssertToken("Trap")
            .HasAmount(8);
    }

    [Fact]
    public async Task TurnStart_CantPlaceTrap_NoUnoccupiedNodesInZone()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [1])
            .AddNode(5, [0], spawnNumber: 2)
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
                    .WithId(2)
                    .WithId(3)
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            LoadoutTemplateBuilder.Foo()
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

        match.AssertToken("Trap")
            .HasAmount(8);
    }

    [Fact]
    public async Task TurnStart_CantPlaceTrap_NoTrapsLeft()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0])
            .AddNode(2, [0])
            .AddNode(3, [0])
            .AddNode(4, [1])
            .AddNode(5, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        var loadout = GetLoadoutBuilder()
            .Build();
        loadout.Fighters[0].Script = loadout.Fighters[0].Script.Replace(":Amount(8)", ":Amount(0)"); // TODO this feels wrong

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                )
                .Build(),
            loadout
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
            .Build(),
            LoadoutTemplateBuilder.Foo()
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

        match.AssertToken("Trap")
            .HasAmount(0);
    }

    [Fact]
    public async Task TrapTrigger_OpponentHero()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .NoExhaustDamage()
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0]) // worker
            .AddNode(2, [0]) // worker
            .AddNode(3, [0]) // worker
            .AddNode(4, [0]) // trap
            .AddNode(5, [0], spawnNumber: 2)
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
                .ConfigStringChoices(c => c
                    .Yes()
                    .No()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4) // trap
                )
                .ConfigPathChoices(c => c
                    .NTimes(4, nc => nc.First())
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                    .First()
                    .First()
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(4)
                )
            .Build(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasCardsInHand(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(4)
            .HasNoTokens();

        match.AssertToken("Trap")
            .HasAmount(7);

        match.AssertFighter("Foo")
            .HasDamage(1);
    }

    [Fact]
    public async Task TrapTrigger_OpponentSidekick()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .NoExhaustDamage()
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0]) // worker
            .AddNode(2, [0]) // worker
            .AddNode(3, [0]) // worker
            .AddNode(4, [0]) // trap
            .AddNode(5, [0]) // bar
            .AddNode(6, [0], spawnNumber: 2)
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
                .ConfigStringChoices(c => c
                    .Yes()
                    .No()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4) // trap
                )
                .ConfigPathChoices(c => c.NTimes(4, nc => nc.First()))
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                    .First()
                    .First()
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Foo")
                    .WithName("Bar")
                )
                .ConfigNodeChoices(c => c
                    .WithId(5)
                )
                .ConfigPathChoices(c => c
                    .First()
                    .FirstStopsAtId(4)
                )
            .Build(),
            LoadoutTemplateBuilder.FooBar()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally();

        match.AssertPlayer(0)
            .SetupCalled()
            .HasCardsInHand(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(4)
            .HasNoTokens();

        match.AssertToken("Trap")
            .HasAmount(7);

        match.AssertFighter("Bar")
            .HasDamage(1);
    }

    [Fact]
    public async Task DeclineTrapTrigger_MainHero()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .NoExhaustDamage()
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0]) // worker
            .AddNode(2, [0]) // worker
            .AddNode(3, [0]) // worker
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(4, [0]) // trap
            .AddNode(5, [0], spawnNumber: 2)
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
                .ConfigStringChoices(c => c
                    .Yes() // place trap
                    .No() // trigger trap
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4) // place trap
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Robert Muldoon")
                    .First()
                    .First()
                    .First()
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(4)
                    .NTimes(3, nc => nc.First())
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
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
            .SetupCalled()
            .HasCardsInHand(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(4)
            .HasToken("Trap");

        match.AssertToken("Trap")
            .HasAmount(7);

        match.AssertFighter("Robert Muldoon")
            .IsAtFullHealth();
    }

    [Fact]
    public async Task DeclineTrapTrigger_MainSidekick()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .NoExhaustDamage()
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0]) // worker
            .AddNode(2, [0]) // worker
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(3, [0]) // worker
            .AddNode(4, [0]) // trap
            .AddNode(5, [0], spawnNumber: 2)
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
                .ConfigStringChoices(c => c
                    .Yes() // place trap
                    .No() // trigger trap
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4) // place trap
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(4)
                    .NTimes(3, nc => nc.First())
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .InNodeWithId(3)
                    .First()
                    .First()
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
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
            .SetupCalled()
            .HasCardsInHand(1)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(4)
            .HasToken("Trap");

        match.AssertToken("Trap")
            .HasAmount(7);

        match.AssertFighter("Robert Muldoon")
            .IsAtFullHealth();
    }

    [Fact]
    public async Task ConfirmTrapTrigger_MainHero()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .NoExhaustDamage()
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0]) // worker
            .AddNode(2, [0]) // worker
            .AddNode(3, [0]) // worker
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(4, [0]) // trap
            .AddNode(5, [0], spawnNumber: 2)
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
                .ConfigStringChoices(c => c
                    .Yes() // place trap
                    .Yes() // trigger trap
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    .WithId(4) // place trap
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(4)
                    .NTimes(3, nc => nc.First())
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .WithName("Robert Muldoon")
                    .First()
                    .First()
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
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
            .SetupCalled()
            .HasCardsInHand(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(4)
            .HasNoTokens();

        match.AssertToken("Trap")
            .HasAmount(7);

        match.AssertFighter("Robert Muldoon")
            .HasDamage(1);
    }

    [Fact]
    public async Task ConfirmTrapTrigger_MainSidekick()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .NoExhaustDamage()
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(1, [0]) // worker
            .AddNode(2, [0]) // worker
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(3, [0]) // worker
            .AddNode(4, [0]) // trap
            .AddNode(5, [0], spawnNumber: 2)
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
                .ConfigStringChoices(c => c
                    .Yes() // place trap
                    .Yes() // trigger trap
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                    .WithId(2)
                    .WithId(3)
                    
                    .WithId(4) // place trap
                )
                .ConfigPathChoices(c => c
                    .FirstStopsAtId(4)
                    .NTimes(3, nc => nc.First())
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .InNodeWithId(3)
                    .First()
                    .First()
                    .First()
                )
                .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d.AddBasicScheme(amount: 10))
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
            .SetupCalled()
            .HasCardsInHand(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertNode(4)
            .HasNoTokens();

        match.AssertToken("Trap")
            .HasAmount(7);
    }

}