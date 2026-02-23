namespace UMServer.Services;

using UMServer.Matches;
using UMServer.Repositories;

public interface IMatchConnectEndpointSerializer
{
    public string Serialize(string clientId, MatchProcess match);
    public (string connectionId, string matchId) Deserialize(string str);
}

public class MatchConnectEndpointSerializer : IMatchConnectEndpointSerializer
{
    private static char SEPARATOR = '@';
    public (string connectionId, string matchId) Deserialize(string str)
    {
        var sp = str.Split(SEPARATOR);
        if (sp.Length != 2)
        {
            return (string.Empty, string.Empty);
        }
        return (sp[0], sp[1]);
    }

    public string Serialize(string clientId, MatchProcess match)
    {
        return $"{clientId}{SEPARATOR}{match.Id}";
    }
}