using UMServer.Matches;

namespace UMServer.Repositories;

public interface IMatchRepository
{
    public IQueryable<MatchProcess> Query();
    void Add(MatchProcess match);
    MatchProcess? Get(string matchId);
    void Remove(MatchProcess match);
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

    public IQueryable<MatchProcess> Query()
    {
        return _matches.AsQueryable();
    }

    public void Remove(MatchProcess match)
    {
        _matches.Remove(match);

        // TODO
    }
}