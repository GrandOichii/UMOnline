using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Shouldly;
using UMDTO;
using UMModel.Models;
using UMServer.BusinessLogic;
using UMServer.Hubs;

namespace UMServer.Tests.BusinessLogic;

public class MatchManagerTests
{
    private (Mock<IHubContext<MatchesHub>>, Mock<IHubClients>, Mock<IClientProxy>) CreateHubMock()
    {
        var hub = new Mock<IHubContext<MatchesHub>>();
        var clients = new Mock<IHubClients>();
        var proxy = new Mock<IClientProxy>();
        clients
            .Setup(_ => _.All)
            .Returns(proxy.Object);
        hub
            .Setup(_ => _.Clients)
            .Returns(clients.Object);

        return (hub, clients, proxy);
    }

    [Fact]
    public async Task ShouldUpdateWatchers()
    {
        // Arrange
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        // Act
        await manager.UpdateWatchers();

        // Assert
        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Once()
        );
    }

    [Fact]
    public async Task ShouldUpdateWatcher()
    {
        // Arrange
        var clientId = "1";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        var (hub, clients, all) = CreateHubMock();

        var single = new Mock<ISingleClientProxy>();
        clients
            .Setup(_ => _.Client(clientId))
            .Returns(single.Object);

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        // Act
        await manager.UpdateWatcher(clientId);

        // Assert
        single.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Once()
        );
        all.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Never()
        );
    }

    [Fact]
    public async Task ShouldNotGetMatch()
    {
        // Arrange
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        var configRepo = new Mock<IMatchConfigRepository>();
        var hub = new Mock<IHubContext<MatchesHub>>();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        // Act
        var match = await manager.Get("1");

        // Assert
        match.ShouldBeNull();
    }

    [Fact]
    public async Task ShouldCreateAndGetMatch()
    {
        // Arrange
        var configName = "config1";
        var ownerId = "1";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        configRepo
            .Setup(_ => _.ByName(configName))
            .ReturnsAsync(config);
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        var createParams = new CreateMatchParams()
        {
            Title = "match1",
            AllowedLoadouts = ["l1", "l2", "l3"],
            MatchConfigName = configName
        };

        // Act
        var created = await manager.Create(
            new ConnectedClient()
            {
                Id = ownerId,
                Name = "Client"  
            },
            createParams
        );
        var match = await manager.Get(created!.Id);

        // Assert
        match.ShouldNotBeNull();
        match.OwnerId.ShouldBe(ownerId);
        match.Config.ShouldBe(config);
        match.CreateParams.ShouldBe(createParams);
        match.MatchException.ShouldBeNull();
        match.Players.Count.ShouldBe(0); // * creator technically connected yet
        match.Record.ShouldBeNull();
        match.Status.ShouldBe(Matches.MatchProcessStatus.WAITING_FOR_PLAYERS);

        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Once()
        );
    }

    [Fact]
    public async Task ShouldNotCreateMatch_ConfigNotFound()
    {
        // Arrange
        var configName = "config1";
        var ownerId = "1";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        configRepo
            .Setup(_ => _.ByName(configName))
            .ReturnsAsync((MatchConfig?)null);
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        var createParams = new CreateMatchParams()
        {
            Title = "match1",
            AllowedLoadouts = ["l1", "l2", "l3"],
            MatchConfigName = configName
        };

        // Act
        var created = await manager.Create(
            new ConnectedClient()
            {
                Id = ownerId,
                Name = "Client"  
            },
            createParams
        );

        // Assert
        created.ShouldBeNull();

        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Never()
        );
    }

    [Fact]
    public async Task ShouldWSConnect()
    {
        // Arrange
        var configName = "config1";
        var ownerId = "1";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        await clientRepo.Add(ownerId, "Client1");
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        configRepo
            .Setup(_ => _.ByName(configName))
            .ReturnsAsync(config);
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        var createParams = new CreateMatchParams()
        {
            Title = "match1",
            AllowedLoadouts = ["l1", "l2", "l3"],
            MatchConfigName = configName
        };

        var ws = Mock.Of<WebSocket>();
        var wsm = new Mock<WebSocketManager>();
        wsm
            .Setup(_ => _.AcceptWebSocketAsync())
            .ReturnsAsync(ws);

        // Act
        var created = await manager.Create(
            new ConnectedClient()
            {
                Id = ownerId,
                Name = "Client"  
            },
            createParams
        );
        created!.MatchEndTask.SetResult();
        await created.ConnectClient((await clientRepo.Get(ownerId))!);
        var errMsg = await manager.WSTryConnect(wsm.Object, ownerId, created!.Id);

        // Assert
        errMsg.ShouldBeEmpty();

        created.Players.Count.ShouldBe(1);
        var player = created.Players[0];
        player.Client.Id.ShouldBe(ownerId);
        player.Loadout.ShouldBeNull();
        player.TeamIdx.ShouldBe(0); // TODO? might change
        player.Socket.ShouldBe(ws);

        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.AtLeast(1)
        );
    }

    [Fact]
    public async Task ShouldNotWSConnect_NotConnectedToMatch()
    {
        // Arrange
        var configName = "config1";
        var ownerId = "1";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        await clientRepo.Add(ownerId, "Client1");
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        configRepo
            .Setup(_ => _.ByName(configName))
            .ReturnsAsync(config);
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        var createParams = new CreateMatchParams()
        {
            Title = "match1",
            AllowedLoadouts = ["l1", "l2", "l3"],
            MatchConfigName = configName
        };

        var ws = Mock.Of<WebSocket>();
        var wsm = new Mock<WebSocketManager>();
        wsm
            .Setup(_ => _.AcceptWebSocketAsync())
            .ReturnsAsync(ws);

        // Act
        var created = await manager.Create(
            new ConnectedClient()
            {
                Id = ownerId,
                Name = "Client"  
            },
            createParams
        );
        created!.MatchEndTask.SetResult();
        var errMsg = await manager.WSTryConnect(wsm.Object, ownerId, created!.Id);

        // Assert
        errMsg.ShouldNotBeEmpty();

        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Once()
        );
    }

    // TODO finish
    // [Fact]
    // public async Task ShouldNotWSConnect_NotAcceptingWSConnections()
    // {
    //     // Arrange
    //     var configName = "config1";
    //     var ownerId = "1";
    //     var matchRepo = new MatchRepository();
    //     var clientRepo = new ClientRepository(
    //         Mock.Of<ILogger<ClientRepository>>()
    //     );
    //     await clientRepo.Add(ownerId, "Client1");
    //     var configRepo = new Mock<IMatchConfigRepository>();
    //     var config = Mock.Of<MatchConfig>();
    //     configRepo
    //         .Setup(_ => _.ByName(configName))
    //         .ReturnsAsync(config);
    //     var (hub, _, proxy) = CreateHubMock();

    //     var manager = new MatchManager(
    //         Mock.Of<ILogger<MatchManager>>(),
    //         matchRepo,
    //         clientRepo,
    //         configRepo.Object,
    //         hub.Object
    //     );

    //     var createParams = new CreateMatchParams()
    //     {
    //         Title = "match1",
    //         AllowedLoadouts = ["l1", "l2", "l3"],
    //         MatchConfigName = configName
    //     };

    //     var ws = Mock.Of<WebSocket>();
    //     var wsm = new Mock<WebSocketManager>();
    //     wsm
    //         .Setup(_ => _.AcceptWebSocketAsync())
    //         .ReturnsAsync(ws);

    //     // Act
    //     var created = await manager.Create(
    //         new ConnectedClient()
    //         {
    //             Id = ownerId,
    //             Name = "Client"  
    //         },
    //         createParams
    //     );
    //     await created!.status
    //     created!.MatchEndTask.SetResult();
    //     var errMsg = await manager.WSTryConnect(wsm.Object, ownerId, created!.Id);

    //     // Assert
    //     errMsg.ShouldNotBeEmpty();

    //     proxy.Verify(
    //         _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
    //         Times.Once()
    //     );
    // }

    [Fact]
    public async Task ShouldNotWSConnect_BeforeOwner()
    {
        // Arrange
        var configName = "config1";
        var ownerId = "1";
        var clientId = "2";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        await clientRepo.Add(ownerId, "Owner1");
        await clientRepo.Add(clientId, "Client1");
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        configRepo
            .Setup(_ => _.ByName(configName))
            .ReturnsAsync(config);
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        var createParams = new CreateMatchParams()
        {
            Title = "match1",
            AllowedLoadouts = ["l1", "l2", "l3"],
            MatchConfigName = configName
        };

        var ws = Mock.Of<WebSocket>();
        var wsm = new Mock<WebSocketManager>();
        wsm
            .Setup(_ => _.AcceptWebSocketAsync())
            .ReturnsAsync(ws);

        // Act
        var created = await manager.Create(
            new ConnectedClient()
            {
                Id = ownerId,
                Name = "Client"  
            },
            createParams
        );
        created!.MatchEndTask.SetResult();
        await created.ConnectClient((await clientRepo.Get(clientId))!);
        var errMsg = await manager.WSTryConnect(wsm.Object, clientId, created!.Id);

        // Assert
        errMsg.ShouldNotBeEmpty();

        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Once()
        );
    }

    [Fact]
    public async Task ShouldNotWSConnect_UnknownMatch()
    {
        // Arrange
        var configName = "config1";
        var clientId = "2";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        await clientRepo.Add(clientId, "Client1");
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        configRepo
            .Setup(_ => _.ByName(configName))
            .ReturnsAsync(config);
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        var createParams = new CreateMatchParams()
        {
            Title = "match1",
            AllowedLoadouts = ["l1", "l2", "l3"],
            MatchConfigName = configName
        };

        var ws = Mock.Of<WebSocket>();
        var wsm = new Mock<WebSocketManager>();
        wsm
            .Setup(_ => _.AcceptWebSocketAsync())
            .ReturnsAsync(ws);

        // Act
        var errMsg = await manager.WSTryConnect(wsm.Object, clientId, "1");

        // Assert
        errMsg.ShouldNotBeEmpty();

        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.Never()
        );
    }

    [Fact]
    public async Task ShouldRemoveMatchesWithRemovedClient()
    {
        // Arrange
        var configName = "config1";
        var ownerId = "1";
        var matchRepo = new MatchRepository();
        var clientRepo = new ClientRepository(
            Mock.Of<ILogger<ClientRepository>>()
        );
        await clientRepo.Add(ownerId, "Client1");
        var configRepo = new Mock<IMatchConfigRepository>();
        var config = Mock.Of<MatchConfig>();
        configRepo
            .Setup(_ => _.ByName(configName))
            .ReturnsAsync(config);
        var (hub, _, proxy) = CreateHubMock();

        var manager = new MatchManager(
            Mock.Of<ILogger<MatchManager>>(),
            matchRepo,
            clientRepo,
            configRepo.Object,
            hub.Object
        );

        var createParams = new CreateMatchParams()
        {
            Title = "match1",
            AllowedLoadouts = ["l1", "l2", "l3"],
            MatchConfigName = configName
        };

        var ws = Mock.Of<WebSocket>();
        var wsm = new Mock<WebSocketManager>();
        wsm
            .Setup(_ => _.AcceptWebSocketAsync())
            .ReturnsAsync(ws);

        // Act
        var client = (await clientRepo.Get(ownerId))!;
        for (int i = 0; i < 3; ++i)
        {
            var created = await manager.Create(
                new ConnectedClient()
                {
                    Id = ownerId,
                    Name = "Client"  
                },
                createParams
            );
            await created!.ConnectClient(client);
        }

        await manager.ProcessRemovedClient(client);
        var matches = matchRepo.All();

        // Assert
        matches.ShouldBeEmpty();

        proxy.Verify(
            _ => _.SendCoreAsync("UpdateTables", It.IsAny<object[]>(), default),
            Times.AtLeast(1)
        );
    }
}