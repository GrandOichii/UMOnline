using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using UMDTO;
using UMServer.Extensions;
using UMServer.Matches;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface IMatchesManager
{
    // Task<MatchProcess?> WebSocketCreate(WebSocketManager ws);
    Task<MatchProcess?> Create(ConnectedClient client, CreateMatchParams createParams);
    Task ProcessRemovedClient(ConnectedClient client);
    Task WSConnect(WebSocket socket, string connectionId, string matchId);
}

public class MatchesManager(
    ILogger<MatchesManager> logger,
    IMatchRepository matchRepo,
    IClientRepository clientRepo,
    ILoadoutRepository loadoutRepo
) : IMatchesManager
{
    // public async Task<MatchProcess?> WebSocketCreate(WebSocketManager ws)
    // {
    //     var socket = await ws.AcceptWebSocketAsync();
    //     await socket.Write("mcp");
    //     var paramsRaw = await socket.Read();
    //     var createParams = JsonSerializer.Deserialize<CreateMatchParams>(paramsRaw);
    //     if (createParams is null)
    //     {
    //         logger.LogDebug("Connected user didn't provide corrent {}, stopping match creation", nameof(CreateMatchParams));
    //         return null;
    //     }

    //     var match = new MatchProcess(
    //         createParams
    //     );
    //     await matchRepo.Add(match);
    //     // var _ = match.Configure();

    //     // await socket.Write($"id:{match.ID}");
    //     // await match.AddWSPlayer(socket);

    //     return match;
    // }

    public async Task<MatchProcess?> Create(ConnectedClient client, CreateMatchParams createParams)
    {
        // TODO checks

        var match = new MatchProcess(
            Guid.NewGuid().ToString(),
            createParams
        );

        matchRepo.Add(match);

        return match;
    }

    public async Task ProcessRemovedClient(ConnectedClient client)
    {
        // TODO either just remove all matches that have this client
        // TODO or replace this user with an AI
    }

    public async Task WSConnect(WebSocket socket, string connectionId, string matchId)
    {
        var client = await clientRepo.GetClient(connectionId);
        if (client is null)
        {
            throw new Exception($"Unregistered client with Id = {connectionId} tried to connect to a match");
        }
        
        var match = matchRepo.Get(matchId);
        if (match is null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid match id", CancellationToken.None);            
            return;
        }

        var loadoutName = await socket.Read();
        var loadout = loadoutRepo
            .Query()
            .Where(l => l.Name == loadoutName)
            .SingleOrDefaultAsync();
        if (loadout is null)
        {
            await socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "Invalid deck", CancellationToken.None);            
            return;
        }

        logger.LogDebug("Client {} wants to connect to match {} using loadout {}", client.Name, match.CreateParams.Title, loadout.Name);
    }
}