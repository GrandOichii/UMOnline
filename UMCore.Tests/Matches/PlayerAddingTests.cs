using System.Threading.Tasks;
using Shouldly;
using UMCore.Tests.Setup;

namespace UMCore.Tests.Matches;

public class PlayerAddingTests
{
    [Fact]
    public async Task CantRunWithoutPlayers()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();
        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedUnintentionally()
            .CantRun();
    }

    [Fact]
    public async Task CantRunWithOnePlayer()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();
        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedUnintentionally()
            .CantRun();
    }

    [Fact]
    public async Task CantAddWithSameLoadout()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();

        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo()
        );

        // Assert
        result.ShouldBeFalse();
        match.Assert()
            .CantRun()
            .PlayerCount(1);
    }

    [Fact]
    public async Task CantAddMoreThanTeamSize()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(1)
            .Build();

        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo1")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo3")
        );

        // Assert
        result.ShouldBeFalse();
        match.Assert()
            .CanRun()
            .PlayerCount(2);
    }

    [Fact]
    public async Task CantStartGameWithUnbalancedTeams()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo3")
        );

        // Assert
        match.Assert()
            .CantRun()
            .PlayerCount(3);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CanRunWithTeamSizes(int teamSize)
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(teamSize)
            .Build();

        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        // Act
        for (int i = 0; i < teamSize; ++i)
        {
            await match.AddMainPlayer(
                TestPlayerControllerBuilder.Crash(),
                LoadoutTemplateBuilder.Foo($"foo{i}")
            );
        }
        for (int i = 0; i < teamSize; ++i)
        {
            await match.AddOpponent(
                TestPlayerControllerBuilder.Crash(),
                LoadoutTemplateBuilder.Foo($"bar{i}")
            );
        }

        // Assert
        match.Assert()
            .CanRun()
            .PlayerCount(teamSize * 2);
    }
}

public class TODOSortTheseTests
{
    [Fact]
    public async Task SetupCalledForPlayers()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();

        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.Run();

        // Assert
        match.Assert()
            .CrashedIntentionally(); // TODO could change

        match.AssertPlayer(0)
            .SetupCalled();
        match.AssertPlayer(1)
            .SetupCalled();
    }

    [Fact]
    public async Task ShouldRunBase()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();

        var mapTemplate = MapTemplateBuilder.BuildDefault();
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
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crash(),
            LoadoutTemplateBuilder.Foo("foo2")
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


}