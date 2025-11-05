using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class WillowTaraTests
{
    private static readonly string IDENTITY_ATTR = "IDENTITY";
    private static readonly string DARK_WILLOW = "Dark Willow";
    private static readonly string WILLOW = "Willow";

    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Willow")
        .Load("../../../../.generated/loadouts/Willow/Willow.json")
        .ClearDeck();

    [Fact]
    public async Task CheckStartingIdentity()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .FirstPlayer(1)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigNodeChoices(c => c.First())
            .Build(),
            GetLoadoutBuilder().Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DeclareWinner()
                    .CrashMatch()
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
            .AttrEq(IDENTITY_ATTR, WILLOW)
            .HasFighterWithName(WILLOW)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

    [Theory]
    [InlineData("Willow")]
    [InlineData("Tara")]
    public async Task DamageTrigger(string damageTo)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DealDamage(damageTo, 1)
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigNodeChoices(c => c
                    .WithId(10)
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
            .SetupCalled()
            .AttrEq(IDENTITY_ATTR, DARK_WILLOW)
            .HasFighterWithName(DARK_WILLOW)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task EOT_DontChange()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(1, [0], spawnNumber: 2)
            .AddNode(2, [0])
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigNodeChoices(c => c.WithId(2))
                .ConfigActions(c => c
                    .DealDamage("Willow", 1)
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c.NTimes(2, nc => nc.First()))
                .ConfigPathChoices(c => c.NTimes(2, nc => nc.First()))
            .Build(),
            GetLoadoutBuilder().Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DeclareWinner()
                    .CrashMatch()
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
            .AttrEq(IDENTITY_ATTR, DARK_WILLOW)
            .HasFighterWithName(DARK_WILLOW)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }
    
    [Fact]
    public async Task EOT_Change()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(0)
            .ActionsPerTurn(1)
            .Build();

        var mapTemplate = new MapTemplateBuilder()
            .AddNode(0, [0], spawnNumber: 1)
            .AddNode(2, [0])
            .AddNode(1, [0], spawnNumber: 2)
            .ConnectAllAsLine()
            .Build();

        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            new TestPlayerControllerBuilder()
                .ConfigNodeChoices(c => c.WithId(2))
                .ConfigActions(c => c
                    .DealDamage("Willow", 1)
                    .Manoeuvre()
                )
                .ConfigFighterChoices(c => c.NTimes(2, nc => nc.First()))
                .ConfigPathChoices(c => c.NTimes(2, nc => nc.First()))
            .Build(),
            GetLoadoutBuilder().Build()
        );
        await match.AddOpponent(
            new TestPlayerControllerBuilder()
                .ConfigActions(c => c
                    .DeclareWinner()
                    .CrashMatch()
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
            .AttrEq(IDENTITY_ATTR, WILLOW)
            .HasFighterWithName(WILLOW)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

    // [Theory]
    // [InlineData(true)]
    // [InlineData(false)]
    // public async Task CheckCardPlayability(bool isJekyll)
    // {
    //     var identityMap = new Dictionary<bool, string>()
    //     {
    //         { true, DR_JEKYLL },
    //         { false, MR_HYDE },
    //     };

    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .InitialHandSize(1)
    //         .ActionsPerTurn(2)
    //         .Build();

    //     var mapTemplate = MapTemplateBuilder.Build2x2();

    //     var match = new TestMatchWrapper(
    //         config,
    //         mapTemplate
    //     );

    //     await match.AddMainPlayer(
    //         new TestPlayerControllerBuilder()
    //             .ConfigActions(c => c
    //                 .Scheme()
    //                 .DeclareWinner()
    //                 .CrashMatch()
    //             )
    //             .ConfigHandCardChoices(c => c
    //                 .First()
    //             )
    //             .ConfigStringChoices(c => c
    //                 .Choose(isJekyll ? "No" : "Yes")
    //             )
    //         .Build(),
    //         GetLoadoutBuilder()
    //             .ConfigDeck(d => d
    //                 .Add(new LoadoutCardTemplateBuilder()
    //                     .Scheme()
    //                     .CanBePlayedBy(identityMap[isJekyll])
    //                 .Build())
    //             )
    //         .Build()
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
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();
    // }

    // [Theory]
    // [InlineData(true)]
    // [InlineData(false)]
    // public async Task CheckCardPlayabilityReverse(bool isJekyll)
    // {
    //     var identityMap = new Dictionary<bool, string>()
    //     {
    //         { true, DR_JEKYLL },
    //         { false, MR_HYDE },
    //     };

    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .InitialHandSize(1)
    //         .ActionsPerTurn(2)
    //         .Build();

    //     var mapTemplate = MapTemplateBuilder.Build2x2();

    //     var match = new TestMatchWrapper(
    //         config,
    //         mapTemplate
    //     );

    //     await match.AddMainPlayer(
    //         new TestPlayerControllerBuilder()
    //             .ConfigActions(c => c
    //                 .Assert(a => a.CantScheme())
    //                 .DeclareWinner()
    //                 .CrashMatch()
    //             )
    //             .ConfigStringChoices(c => c
    //                 .Choose(isJekyll ? "No" : "Yes")
    //             )
    //         .Build(),
    //         GetLoadoutBuilder()
    //             .ConfigDeck(d => d
    //                 .Add(new LoadoutCardTemplateBuilder()
    //                     .Scheme()
    //                     .CanBePlayedBy(identityMap[!isJekyll])
    //                 .Build())
    //             )
    //         .Build()
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
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();
    // }

    // [Fact]
    // public async Task CheckManoeuvreTriggerJekyll()
    // {
    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .InitialHandSize(0)
    //         .ActionsPerTurn(2)
    //         .Build();

    //     var mapTemplate = MapTemplateBuilder.Build2x2();

    //     var match = new TestMatchWrapper(
    //         config,
    //         mapTemplate
    //     );

    //     await match.AddMainPlayer(
    //         new TestPlayerControllerBuilder()
    //             .ConfigActions(c => c
    //                 .Manoeuvre()
    //                 .DeclareWinner()
    //                 .CrashMatch()
    //             )
    //             .ConfigHandCardChoices(c => c
    //                 .Nothing()
    //             )
    //             .ConfigFighterChoices(c => c
    //                 .First()
    //             )
    //             .ConfigPathChoices(c => c
    //                 .First()
    //             )
    //             .ConfigStringChoices(c => c
    //                 .No()
    //             )
    //         .Build(),
    //         GetLoadoutBuilder().ConfigDeck(d => d.AddBasicScheme(amount: 10)).Build()
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
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();

    //     match.AssertFighter(FIGHTER_KEY)
    //         .IsAtFullHealth();
    // }

    // [Fact]
    // public async Task CheckManoeuvreTriggerHyde()
    // {
    //     // Arrange
    //     var config = new MatchConfigBuilder()
    //         .InitialHandSize(0)
    //         .ActionsPerTurn(2)
    //         .Build();

    //     var mapTemplate = MapTemplateBuilder.Build2x2();

    //     var match = new TestMatchWrapper(
    //         config,
    //         mapTemplate
    //     );

    //     await match.AddMainPlayer(
    //         new TestPlayerControllerBuilder()
    //             .ConfigActions(c => c
    //                 .Manoeuvre()
    //                 .DeclareWinner()
    //                 .CrashMatch()
    //             )
    //             .ConfigHandCardChoices(c => c
    //                 .Nothing()
    //             )
    //             .ConfigFighterChoices(c => c
    //                 .First()
    //             )
    //             .ConfigPathChoices(c => c
    //                 .First()
    //             )
    //             .ConfigStringChoices(c => c
    //                 .Yes()
    //             )
    //         .Build(),
    //         GetLoadoutBuilder().ConfigDeck(d => d.AddBasicScheme(amount: 10)).Build()
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
    //         .IsWinner();
    //     match.AssertPlayer(1)
    //         .SetupCalled()
    //         .IsNotWinner();

    //     match.AssertFighter(FIGHTER_KEY)
    //         .HasDamage(1);
    // }


}
