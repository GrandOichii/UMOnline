using Shouldly;
using UMModel.Models;
using UMServer.BusinessLogic;

namespace UMServer.Tests.BusinessLogic;

public class UpdateManagerTests
{
    [Fact]
    public async Task ShouldGetCurrent()
    {
        // Arrange
        var data = "data";
        var repo = new Mock<IContentUpdateRepository>();
        repo
            .Setup(_ => _.Active())
            .ReturnsAsync(new ContentUpdate()
            {
                CreatedDT = DateTime.Now,
                Data = data,
                Description = "",
                Id = 1,
                IsActive = true
            });

        var manager = new UpdateManager(repo.Object);

        // Act
        var result = await manager.Current();

        // Assert
        result.ShouldBe(data);
    }
}