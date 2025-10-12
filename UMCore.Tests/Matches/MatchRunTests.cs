using System.Threading.Tasks;
using Shouldly;
using UMCore.Tests.Setup;

namespace UMCore.Tests;

public class MatchRunTests
{
    [Fact]
    public async Task CantRunWithoutPlayers()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();
        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatch(
            config,
            mapTemplate
        );

        // Act
        await match.Run();

        // Assert
        match.Exception.ShouldNotBeNull();
        match.Match.CanRun().ShouldBeFalse();
    }

    [Fact]
    public async Task CantRunWithOnePlayer()
    {
        // Arrange
        var config = MatchConfigBuilder.BuildDefault();
        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatch(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.AutoPass(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Exception.ShouldNotBeNull();
        match.Match.CanRun().ShouldBeFalse();
    }

    [Fact]
    public async Task CantAddWithSameLoadout()
    {
        // Arrange
        var config = new MatchConfigBuilder()
            .TeamSize(1)
            .Build();

        var mapTemplate = MapTemplateBuilder.BuildDefault();
        var match = new TestMatch(
            config,
            mapTemplate
        );

        await match.AddMainPlayer(
            TestPlayerControllerBuilder.AutoPass(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        var result = await match.AddOpponent(
            TestPlayerControllerBuilder.AutoPass(),
            LoadoutTemplateBuilder.Foo()
        );

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public async Task CantStartMatchWithInbalancedTeams()
    {
        // TODO
    }
}