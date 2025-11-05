using System.Text.Json;

namespace UMCore.Tests.Fighters;

public class JekyllHydeTests
{
    private static readonly string IDENTITY_ATTR = "IDENTITY";
    private static readonly string DR_JEKYLL = "Dr. Jekyll";
    private static readonly string MR_HYDE = "Mr. Hyde";
    private static readonly string FIGHTER_KEY = DR_JEKYLL;

    private static LoadoutTemplateBuilder GetLoadoutBuilder() => new LoadoutTemplateBuilder("Jekyll & Hyde")
        .Load("../../../../.generated/loadouts/Jekyll & Hyde/Jekyll & Hyde.json")
        .ClearDeck();

    // [Theory]
    // [InlineData("Dr. Jekyll")]
    // [InlineData("Mr. Hyde")]
    //     await match.AddMainPlayer(
    //     new TestPlayerControllerBuilder()
    //         .ConfigActions(a => a
    //             .DeclareWinner()
    //             .CrashMatch()
    //         )
    //         .ConfigStringChoices(c => c
    //             .Assert(a => a
    //                 .EquivalentTo(["BIG", "SMALL"])
    //             )
    //             .Choose(targetSize)
    //         )
    //         .ConfigNodeChoices(c => c
    //             .WithId(1)
    //         )
    //         .Build(),
    //     GetLoadoutBuilder().Build()
    // );
    // await match.AddOpponent(
    //     TestPlayerControllerBuilder.Crasher(),
    //     LoadoutTemplateBuilder.Foo()
    // );

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
            TestPlayerControllerBuilder.Crasher(),
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
            .AttrEq(IDENTITY_ATTR, DR_JEKYLL)
            .HasFighterWithName(DR_JEKYLL)
            .IsNotWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsWinner();
    }

    [Theory]
    [InlineData("No", "Dr. Jekyll")]
    [InlineData("Yes", "Mr. Hyde")]
    public async Task Check(string changeIdentity, string expectedIdentity)
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
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Choose(changeIdentity)
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
            .AttrEq(IDENTITY_ATTR, expectedIdentity)
            .HasFighterWithName(expectedIdentity)
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckCardPlayability(bool isJekyll)
    {
        var identityMap = new Dictionary<bool, string>()
        {
            { true, DR_JEKYLL },
            { false, MR_HYDE },
        };

        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Scheme()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .First()
                )
                .ConfigStringChoices(c => c
                    .Choose(isJekyll ? "No" : "Yes")
                )
            .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
                        .Scheme()
                        .CanBePlayedBy(identityMap[isJekyll])
                    .Build())
                )
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CheckCardPlayabilityReverse(bool isJekyll)
    {
        var identityMap = new Dictionary<bool, string>()
        {
            { true, DR_JEKYLL },
            { false, MR_HYDE },
        };

        // Arrange
        var config = new MatchConfigBuilder()
            .InitialHandSize(1)
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
                    .Assert(a => a.CantScheme())
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigStringChoices(c => c
                    .Choose(isJekyll ? "No" : "Yes")
                )
            .Build(),
            GetLoadoutBuilder()
                .ConfigDeck(d => d
                    .Add(new LoadoutCardTemplateBuilder()
                        .Scheme()
                        .CanBePlayedBy(identityMap[!isJekyll])
                    .Build())
                )
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();
    }

    [Fact]
    public async Task CheckManoeuvreTriggerJekyll()
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
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First()
                )
                .ConfigStringChoices(c => c
                    .No()
                )
            .Build(),
            GetLoadoutBuilder().ConfigDeck(d => d.AddBasicScheme(amount: 10)).Build()
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter(FIGHTER_KEY)
            .IsAtFullHealth();
    }

    [Fact]
    public async Task CheckManoeuvreTriggerHyde()
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
                    .Manoeuvre()
                    .DeclareWinner()
                    .CrashMatch()
                )
                .ConfigHandCardChoices(c => c
                    .Nothing()
                )
                .ConfigFighterChoices(c => c
                    .First()
                )
                .ConfigPathChoices(c => c
                    .First()
                )
                .ConfigStringChoices(c => c
                    .Yes()
                )
            .Build(),
            GetLoadoutBuilder().ConfigDeck(d => d.AddBasicScheme(amount: 10)).Build()
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
            .IsWinner();
        match.AssertPlayer(1)
            .SetupCalled()
            .IsNotWinner();

        match.AssertFighter(FIGHTER_KEY)
            .HasDamage(1);
    }


}
