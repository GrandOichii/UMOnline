using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Shouldly;
using UMDTO;
using UMServer.BusinessLogic;
using UMServer.Hubs;
using UMServer.Matches;
using UMServer.Services;

namespace UMServer.Tests.Hubs;

public class MatchesHubTests
{
    [Fact]
    public async Task ShouldAddToClientsOnConnected()
    {
        // Arrange
        var connectionId = "1";
        var matchManager = new Mock<IMatchManager>();
        var clientRepo = new Mock<IClientRepository>();
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        await hub.OnConnectedAsync();

        // Assert
        matchManager.Verify(
            _ => _.UpdateWatcher(connectionId),
            Times.Once()
        );
        clientRepo.Verify(
            _ => _.Add(connectionId, It.IsAny<string>()),
            Times.Never()
        );
    }

    [Fact]
    // public async Task ShouldRemoveFromClientsOnDisconnect()
    public async Task ShouldDoNothingOnUnknownDisconnect()
    {
        // Arrange
        var connectionId = "1";
        var client = new ConnectedClient()
        {
            Id = connectionId,
            Name = "Name"
        };
        var matchManager = new Mock<IMatchManager>();
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Get(connectionId))
            .ReturnsAsync((ConnectedClient?)null);
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        await hub.OnDisconnectedAsync(null);

        // Assert
        matchManager.Verify(
            _ => _.ProcessRemovedClient(It.IsAny<ConnectedClient>()),
            Times.Never()
        );
        clientRepo.Verify(
            _ => _.Remove(It.IsAny<ConnectedClient>()),
            Times.Never()
        );
    }

    [Fact]
    public async Task ShouldRemoveFromClientsOnDisconnect()
    {
        // Arrange
        var connectionId = "1";
        var client = new ConnectedClient()
        {
            Id = connectionId,
            Name = "Name"
        };
        var matchManager = new Mock<IMatchManager>();
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Get(connectionId))
            .ReturnsAsync(client);
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        await hub.OnDisconnectedAsync(null);

        // Assert
        matchManager.Verify(
            _ => _.ProcessRemovedClient(client),
            Times.Once()
        );
        clientRepo.Verify(
            _ => _.Remove(client),
            Times.Once()
        );
    }

    [Fact]
    public async Task ShouldUpdateMe()
    {
        // Arrange
        var connectionId = "1";
        var client = new ConnectedClient()
        {
            Id = connectionId,
            Name = "Name"
        };
        var matchManager = new Mock<IMatchManager>();
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Get(connectionId))
            .ReturnsAsync(client);
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        await hub.UpdateMe();

        // Assert
        matchManager.Verify(
            _ => _.UpdateWatcher(connectionId),
            Times.Once()  
        );
        matchManager.Verify(
            _ => _.UpdateWatchers(),
            Times.Never()  
        );
    }

    [Fact]
    public async Task ShouldRegisterName()
    {
        // Arrange
        var connectionId = "1";
        var name = "name";
        var matchManager = new Mock<IMatchManager>();
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Add(connectionId, name))
            .ReturnsAsync(string.Empty);
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        var errMsg = await hub.RegisterName(name);

        // Assert
        errMsg.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldNotRegisterName()
    {
        // Arrange
        var connectionId = "1";
        var name = "name";
        var errMsg = "err";
        var matchManager = new Mock<IMatchManager>();
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Add(connectionId, name))
            .ReturnsAsync(errMsg);
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        var resultErrMsg = await hub.RegisterName(name);

        // Assert
        resultErrMsg.ShouldBe(errMsg);
    }

    [Fact]
    public async Task ShouldCreateMatch()
    {
        // Arrange
        var connectionId = "1";
        // var errMsg = "err";
        var matchId = "2";
        var match = new MatchProcess(matchId, connectionId, null!, null!);
        var matchManager = new Mock<IMatchManager>();
        matchManager
            .Setup(_ => _.Create(It.IsAny<ConnectedClient>(), It.IsAny<CreateMatchParams>()))
            .ReturnsAsync(match);
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Get(connectionId))
            .ReturnsAsync(new ConnectedClient()
            {
                Id = connectionId,
                Name = "name"
            });
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        var result = await hub.CreateMatch(Mock.Of<CreateMatchParams>());

        // Assert
        result.ShouldBe(matchId);
    }

    [Fact]
    public async Task ShouldNotCreateMatch_NotConnected()
    {
        // Arrange
        var connectionId = "1";
        // var errMsg = "err";
        var matchId = "2";
        var match = new MatchProcess(matchId, connectionId, null!, null!);
        var matchManager = new Mock<IMatchManager>();
        matchManager
            .Setup(_ => _.Create(It.IsAny<ConnectedClient>(), It.IsAny<CreateMatchParams>()))
            .ReturnsAsync(match);
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Get(connectionId))
            .ReturnsAsync((ConnectedClient?)null);
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        var result = await hub.CreateMatch(Mock.Of<CreateMatchParams>());

        // Assert
        result.ShouldNotBe(matchId);
    }
    
    [Fact]
    public async Task ShouldNotCreateMatch_FailedToCreate()
    {
        // Arrange
        var connectionId = "1";
        // var errMsg = "err";
        var matchId = "2";
        var matchManager = new Mock<IMatchManager>();
        matchManager
            .Setup(_ => _.Create(It.IsAny<ConnectedClient>(), It.IsAny<CreateMatchParams>()))
            .ReturnsAsync((MatchProcess?)null);
        var clientRepo = new Mock<IClientRepository>();
        clientRepo
            .Setup(_ => _.Get(connectionId))
            .ReturnsAsync(new ConnectedClient()
            {
                Id = connectionId,
                Name = "name"
            });
        var loadoutRepo = new Mock<ILoadoutRepository>();
        var coreRepo = new Mock<ICoreScriptRepository>();

        var hubCtx = new Mock<HubCallerContext>();
        var hub = new MatchesHub(
            Mock.Of<ILogger<MatchesHub>>(),
            matchManager.Object,
            clientRepo.Object,
            loadoutRepo.Object,
            coreRepo.Object,
            new MatchConnectEndpointSerializer()
        )
        {
            Context = hubCtx.Object
        };

        hubCtx
            .Setup(_ => _.ConnectionId)
            .Returns(connectionId);

        // Act
        var result = await hub.CreateMatch(Mock.Of<CreateMatchParams>());

        // Assert
        result.ShouldNotBe(matchId);
    }


}
