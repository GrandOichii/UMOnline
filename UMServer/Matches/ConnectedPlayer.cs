using System.Net.WebSockets;
using UMModel.Models;
using UMServer.Repositories;

namespace UMServer.Matches;

public class ConnectedPlayer
{
    public int TeamIdx { get; private set; }
    public Loadout? Loadout { get; private set; }
    public ConnectedClient Client { get; }
    public WebSocket? Socket { get; private set; }

    public ConnectedPlayer(
        ConnectedClient client
    )
    {
        Client = client;
        TeamIdx = 0;
        Loadout = null;
    }

    public void SetTeamIdx(int value)
    {
        TeamIdx = value;
    }

    public void SetLoadout(Loadout value)
    {
        Loadout = value;
    }

    public async Task SetSocket(WebSocket socket)
    {
        Socket = socket;
    }
}