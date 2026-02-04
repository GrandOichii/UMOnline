using System.Text.Json;
using UMDTO;
using UMServer.Extensions;
using UMServer.Matches;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface IMatchesManager
{
    Task<MatchProcess?> WebSocketCreate(WebSocketManager ws);
}

public class MatchesManager(
    ILogger<MatchesManager> logger,
    IMatchRepository matchRepo
) : IMatchesManager
{
    public async Task<MatchProcess?> WebSocketCreate(WebSocketManager ws)
    {
        var socket = await ws.AcceptWebSocketAsync();
        await socket.Write("mcp");
        var paramsRaw = await socket.Read();
        var createParams = JsonSerializer.Deserialize<CreateMatchParams>(paramsRaw);
        if (createParams is null)
        {
            logger.LogDebug("Connected user didn't provide corrent {}, stopping match creation", nameof(CreateMatchParams));
            return null;
        }

        var match = new MatchProcess(
            createParams
        );
        await matchRepo.Add(match);
        // var _ = match.Configure();

        // await socket.Write($"id:{match.ID}");
        // await match.AddWSPlayer(socket);
        
        return match;
    }
}