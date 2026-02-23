using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using UMServer.Extensions;

namespace UMServer.Matches;

public class WebSocketIOHandler(WebSocket socket) : UMCore.Matches.Players.IIOHandler
{
    private readonly WebSocket _socket = socket;

    public async Task Close()
    {
        await _socket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            "MatchEnd",
            CancellationToken.None
        );
    }

    public async Task<string> Read()
    {
        return await _socket.Read();
    }

    public async Task Write(UMCore.Matches.Players.UpdateInfo info)
    {
        await _socket.Write(JsonSerializer.Serialize(info));
    }
}