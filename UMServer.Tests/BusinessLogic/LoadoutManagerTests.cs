using Shouldly;
using UMModel.Models;
using UMServer.BusinessLogic;

namespace UMServer.Tests.BusinessLogic;

public class LoadoutManagerTests
{
    [Fact]
    public async Task ShouldFetchOnlyPublic()
    {
        // Arrange
        var repo = new Mock<ILoadoutRepository>();
        repo
            .Setup(_ => _.AllPublic())
            .ReturnsAsync([
                new LoadoutBuilder()
                    .Build(),
                new LoadoutBuilder()
                    .Build(),
                new LoadoutBuilder()
                    .Build(),
            ]);

        var manager = new LoadoutManager(repo.Object);

        // Act
        var result = await manager.AllLoadouts();

        // Assert
        result.Count().ShouldBe(3);
    }
}