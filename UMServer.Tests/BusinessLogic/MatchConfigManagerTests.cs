using Shouldly;
using UMModel.Models;
using UMServer.BusinessLogic;

namespace UMServer.Tests.BusinessLogic;

public class MatchConfigManagerTests
{
    [Fact]
    public async Task ShouldFetchActive()
    {
        // Arrange
        var repo = new Mock<IMatchConfigRepository>();
        repo
            .Setup(_ => _.All())
            .ReturnsAsync(MatchConfig.GetDefaultData());

        var manager = new MatchConfigManager(repo.Object);

        // Act
        var result = await manager.All();

        // Assert
        result.Count().ShouldBe(MatchConfig.GetDefaultData().Length);
    }
}