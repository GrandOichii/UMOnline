using UMServer.Matches;

namespace UMServer.Repositories;

public interface IMatchRepository
{
    void Add(MatchProcess match);
    MatchProcess? Get(string matchId);
    void Remove(MatchProcess match);
    IEnumerable<MatchProcess> All();
    int Count();
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
        return _matches.SingleOrDefault(m => m.Id == matchId);
    }

    public IEnumerable<MatchProcess> All()
    {
        return _matches;
    }

    public int Count() => _matches.Count;

    public void Remove(MatchProcess match)
    {
        _matches.Remove(match);

        // TODO
    }
}