using Shouldly;
using UMModel.Models;
using UMServer.BusinessLogic;

namespace UMServer.Tests.BusinessLogic;

public class CoreManagerTests
{
    [Fact]
    public async Task ShouldFetchActive()
    {
        // Arrange
        var script = "script";
        var repo = new Mock<ICoreScriptRepository>();
        repo
            .Setup(_ => _.Active())
            .ReturnsAsync(new CoreScript()
            {
                CreatedAt = DateTime.Now,
                IsActive = true,
                Script = script
            });

        var manager = new CoreScriptManager(repo.Object);

        // Act
        var result = await manager.Active();

        // Assert
        result.ShouldBe(script);
    }
}