using Shouldly;

namespace UMCore.Tests.Mechanics;

public class PlayerAddingTests
{
    // TODO add test for MatchConfig.TeamCount
    [Fact]
    public async Task CantRunWithoutPlayers()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();
        var mapTemplate = MapTemplateBuilder.Build2x2();
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
        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crasher(),
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

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo1")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
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

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo1")
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo2")
        );

        // Act
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            LoadoutTemplateBuilder.Foo("foo3")
        );

        // Assert
        match.Assert()
            .CantRun()
            .PlayerCount(3);
    }

    [Fact]
    public async Task CantStartGameLoadoutIncompatibility1()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("Foo1")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .CantBePlayedWith("Foo2")
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("Foo2")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
                    .Build()
                )
                .Build()
        );

        // Act

        // Assert
        match.Assert()
            .CantRun()
            .PlayerCount(1);
    }

    [Fact]
    public async Task CantStartGameLoadoutIncompatibility2()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(2)
            .Build();

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("Foo1")
                .AddFighter(new FighterTemplateBuilder("Foo1", "Foo1")
                    .Build()
                )
                .Build()
        );
        await match.AddOpponent(
            TestPlayerControllerBuilder.Crasher(),
            new LoadoutTemplateBuilder("Foo2")
                .AddFighter(new FighterTemplateBuilder("Foo2", "Foo2")
                    .Build()
                )
                .CantBePlayedWith("Foo1")
                .Build()
        );

        // Act

        // Assert
        match.Assert()
            .CantRun()
            .PlayerCount(1);
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

        var mapTemplate = MapTemplateBuilder.Build2x2();
        var match = new TestMatchWrapper(
            config,
            mapTemplate
        );

        // Act
        for (int i = 0; i < teamSize; ++i)
        {
            await match.AddMainPlayer(
                TestPlayerControllerBuilder.Crasher(),
                LoadoutTemplateBuilder.Foo($"foo{i}")
            );
        }
        for (int i = 0; i < teamSize; ++i)
        {
            await match.AddOpponent(
                TestPlayerControllerBuilder.Crasher(),
                LoadoutTemplateBuilder.Foo($"bar{i}")
            );
        }

        // Assert
        match.Assert()
            .CanRun()
            .PlayerCount(teamSize * 2);
    }
}