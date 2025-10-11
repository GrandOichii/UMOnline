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
            TestPlayerControllerBuilder.BuildDefault(),
            LoadoutTemplateBuilder.Foo()
        );

        // Act
        await match.Run();

        // Assert
        match.Exception.ShouldNotBeNull();
        match.Match.CanRun().ShouldBeFalse();
    }
}