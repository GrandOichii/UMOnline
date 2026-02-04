using UMServer.Matches;

namespace UMServer.Repositories;

public interface IMatchRepository
{
    Task Add(MatchProcess match);
}

public class MatchRepository : IMatchRepository
{
    public Task Add(MatchProcess match)
    {
        throw new NotImplementedException();
    }
}