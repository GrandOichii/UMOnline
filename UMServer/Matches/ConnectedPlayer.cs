using System.Net.WebSockets;
using UMModel.Models;

namespace UMServer.Matches;

public class ConnectedPlayer
{
    public int TeamIdx { get; private set; }
    public Loadout? Loadout { get; private set; }
    public string ClientId { get; }
    public WebSocket? Socket { get; private set; }

    public ConnectedPlayer(
        string clientId
    )
    {
        ClientId = clientId;
        TeamIdx = 0;
        Loadout = null;
    }

    public async Task SetTeamIdx(int value)
    {
        TeamIdx = value;

        // TODO update tables
    }

    public async Task SetLoadout(Loadout value)
    {
        Loadout = value;

        // TODO update tables
    }

    public async Task SetSocket(WebSocket socket)
    {
        Socket = socket;
    }
}