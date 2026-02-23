using Microsoft.AspNetCore.SignalR;
using UMDTO;
using UMServer.BusinessLogic;
using UMServer.Repositories;
using UMServer.Services;

namespace UMServer.Hubs;

public class ConnectionHub(
    ILogger<ConnectionHub> logger,
    IClientRepository clientRepo,
    IMatchesManager matchesManager,
    IMatchConnectEndpointSerializer connectSerializer
) : Hub
{
    // public async Task Amogus()
    // {
    //     await Clients.All.SendAsync("Receive", "amogus");
    // }    

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        logger.LogDebug("New connection! ConnectionId: {}. Waiting for name", Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);

        logger.LogDebug("Client with Id = {} was disconnected", Context.ConnectionId);

        var user = await clientRepo.GetClient(Context.ConnectionId);
        await clientRepo.RemoveClient(Context.ConnectionId);
        await matchesManager.ProcessRemovedClient(user);
    }

    public async Task RegisterName(string name)
    {
        logger.LogDebug("Client with Id = {} tries to register with name {}", Context.ConnectionId, name);

        var errMsg = await clientRepo.AddClient(Context.ConnectionId, name);

        if (string.IsNullOrEmpty(errMsg))
        {
            return;
        }

        await Clients.Client(Context.ConnectionId).SendAsync("RegistrationError", errMsg);
    }

    public async Task<string> CreateMatch(CreateMatchParams createParams)
    {
        var creator = await clientRepo.GetClient(Context.ConnectionId);
        if (creator is null)
        {
            return "err:You are not registered (somehow)";
        }

        logger.LogDebug("User {} wants to create a match with title {}", creator.Name, createParams.Title);

        var match = await matchesManager.Create(creator, createParams);
        // TODO add match

        if (match is null)
        {
            return "err:Failed to create match";
        }

        return connectSerializer.Serialize(creator, match);
    }
}