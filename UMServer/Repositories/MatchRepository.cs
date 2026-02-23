using UMServer.Matches;

namespace UMServer.Repositories;

public interface IMatchRepository
{
    void Add(MatchProcess match);
    MatchProcess? Get(string matchId);
}

public class MatchRepository : IMatchRepository
{
    private List<MatchProcess> _matches = [];

    public void Add(MatchProcess match)
    {
        _matches.Add(match);

        // TODO
    }

    public MatchProcess? Get(string matchId)
    {
        System.Console.WriteLine(_matches.Count);
        System.Console.WriteLine(string.Join("\n", _matches.Select(m => m.Id)));
        return _matches.SingleOrDefault(m => m.Id == matchId);
    }
}