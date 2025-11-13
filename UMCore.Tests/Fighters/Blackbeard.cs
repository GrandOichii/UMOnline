using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class BlackbeardTests
{
    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Blackbeard")
        .Load("../../../../.generated/loadouts/Blackbeard/Blackbeard.json")
        .ClearDeck();

    [Fact]
    public async Task TurnStart_DeclineTrigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Blackbeard
            .AddNode(1, [0])                 // Seadog 1
            .AddNode(2, [0])                 // Seadog 2
            .AddNode(3, [0], spawnNumber: 2) // Foo
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
                .ConfigStringChoices(c => c
                    .No()
                )
                .ConfigNodeChoices(c => c
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
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
            .IntAttrEq("DOUBLOONS", 2)
            .HasUnspentActions(2)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task TurnStart_AcceptTrigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Blackbeard
            .AddNode(1, [0])                 // Seadog 1
            .AddNode(2, [0])                 // Seadog 2
            .AddNode(3, [0], spawnNumber: 2) // Foo
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
                .ConfigStringChoices(c => c
                    .Yes()
                )
                .ConfigNodeChoices(c => c
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
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
            .IntAttrEq("DOUBLOONS", 1)
            .HasUnspentActions(3)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task TurnStart_CantTrigger()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(1)
            .FirstPlayer(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 2) // Blackbeard
            .AddNode(1, [0])                 // Seadog 1
            .AddNode(2, [0])                 // Seadog 2
            .AddNode(3, [0], spawnNumber: 1) // Foo
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
                .ConfigNodeChoices(c => c
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
                .Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .SetIntAttr(0, "DOUBLOONS", 0)
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c.First())
                .ConfigPathChoices(c => c.First())
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
            .IntAttrEq("DOUBLOONS", 0)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2, 1, 1)]
    [InlineData(3, 1, 1)]
    [InlineData(1, 2, 0)]
    [InlineData(2, 2, 0)]
    [InlineData(3, 2, 0)]
    [InlineData(1, 3, 0)]
    [InlineData(2, 3, 0)]
    [InlineData(3, 3, 0)]
    public async Task DamageTrigger(int damageAmount, int damageTimes, int expectedDoubloonCount)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1) // Blackbeard
            .AddNode(1, [0])                 // Seadog 1
            .AddNode(2, [0])                 // Seadog 2
            .AddNode(3, [0], spawnNumber: 2) // Foo
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(a => a
                    .NTimes(damageTimes, nc => nc.DealDamage("Blackbeard", damageAmount))
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(1)
                )
                .ConfigStringChoices(c => c
                    .No()
                )
                .ConfigNodeChoices(c => c
                    .WithId(2)
                )
                .Build(),
            GetLoadoutBuilder()
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
            .IntAttrEq("DOUBLOONS", expectedDoubloonCount)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    // [Theory]
    // [InlineData("Beowulf", 1, 2)]
    // [InlineData("Beowulf", 2, 2)]
    // [InlineData("Beowulf", 3, 2)]
    // [InlineData("Wiglaf", 1, 1)]
    // [InlineData("Wiglaf", 2, 1)]
    // [InlineData("Wiglaf", 3, 1)]
    // public async Task CheckRageOnDamage(string damageTo, int damage, int expectedRage)
    // {
    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .InitialHandSize(0)
    //         .ActionsPerTurn(2)
    //         .Build();

    //     var mapTemplate = new MapTemplateBuilder()
    //         .AddNode(0, [0], spawnNumber: 1) // Beowulf
    //         .AddNode(1, [0])                 // Wiglaf
    //         .AddNode(2, [0], spawnNumber: 2) // Foo
    //         .ConnectAllAsLine()
    //         .Build();

    //     var match = new TestMatchWrapper(
    //         config,
    //         mapTemplate
    //     );

    //     await match.AddMainPlayer(
    //         new TestPlayerControllerBuilder()
    //             .ConfigActions(a => a
    //                 .DealDamage(damageTo, damage)
    //                 .DeclareWinner()
    //                 .CrashMatch()
    //             )
    //             .ConfigNodeChoices(c => c
    //                 .WithId(1)
    //             )
    //             .Build(),
    //         GetLoadoutBuilder()
    //             .Build()
    //     );
    //     await match.AddOpponent(
    //         TestPlayerControllerBuilder.Crasher(),
    //         LoadoutTemplateBuilder.Foo()
    //     );

    //     // Act
    //     await match.Run();

    //     // Assert
    //     match.Assert()
    //         .CrashedIntentionally();

    //     match.AssertPlayer(0)
    //         .SetupCalled()
    //         .IntAttrEq("RAGE", expectedRage)
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();
    // }

    // [Fact]
    // public async Task CheckRageCap()
    // {
    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .InitialHandSize(0)
    //         .ActionsPerTurn(2)
    //         .Build();

    //     var mapTemplate = new MapTemplateBuilder()
    //         .AddNode(0, [0], spawnNumber: 1) // Beowulf
    //         .AddNode(1, [0])                 // Wiglaf
    //         .AddNode(2, [0], spawnNumber: 2) // Foo
    //         .ConnectAllAsLine()
    //         .Build();

    //     var match = new TestMatchWrapper(
    //         config,
    //         mapTemplate
    //     );

    //     await match.AddMainPlayer(
    //         new TestPlayerControllerBuilder()
    //             .ConfigActions(a => a
    //                 .NTimes(10, nc => nc
    //                     .DealDamage("Beowulf", 1)
    //                 )
    //                 .DeclareWinner()
    //                 .CrashMatch()
    //             )
    //             .ConfigNodeChoices(c => c
    //                 .WithId(1)
    //             )
    //             .Build(),
    //         GetLoadoutBuilder()
    //             .Build()
    //     );
    //     await match.AddOpponent(
    //         TestPlayerControllerBuilder.Crasher(),
    //         LoadoutTemplateBuilder.Foo()
    //     );

    //     // Act
    //     await match.Run();

    //     // Assert
    //     match.Assert()
    //         .CrashedIntentionally();

    //     match.AssertPlayer(0)
    //         .SetupCalled()
    //         .IntAttrEq("RAGE", 3)
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();
    // }
}
