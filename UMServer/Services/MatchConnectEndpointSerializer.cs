namespace UMServer.Services;

using UMServer.Matches;
using UMServer.Repositories;

public interface IMatchConnectEndpointSerializer
{
    public string Serialize(ConnectedClient client, MatchProcess match);
    public (string connectionId, string matchId) Deserialize(string str);
}

public class MatchConnectEndpointSerializer : IMatchConnectEndpointSerializer
{
    public (string connectionId, string matchId) Deserialize(string str)
    {
        var sp = str.Split("_");
        if (sp.Length != 2)
        {
            return (string.Empty, string.Empty);
        }
        return (sp[0], sp[1]);
    }

    public string Serialize(ConnectedClient client, MatchProcess match)
    {
        return $"{client.ConnectionId}_{match.Id}";
    }
}