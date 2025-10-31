namespace UMCore.Tests.Fighters;

public class InGenTests
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
                    .NTimes(3, nc => nc.First())
                    .NTimes(3, nc => nc.First())
                    .NTimes(3, nc => nc.First())
                    .NTimes(3, nc => nc.First())
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
                .ConfigNodeChoices(c => c
                    .WithId(4) // should stop after first
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
            // .HasCardsInHand(2) // TODO add back
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        // match.AssertNode(4) // TODO add back
        //     .HasNoTokens();

        match.AssertToken("Trap")
            .HasAmount(7);

        match.AssertFighter("Foo")
            .HasDamage(1);
    }

}