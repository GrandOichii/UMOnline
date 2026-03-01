using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface IMatchConfigRepository
{
    public Task<IEnumerable<MatchConfig>> All();
    public Task<MatchConfig?> ByName(string name);
}

public class MatchConfigRepository(UMContext ctx) : IMatchConfigRepository
{
    public async Task<IEnumerable<MatchConfig>> All() {
        return Query()
            .AsEnumerable();
    }

    private IQueryable<MatchConfig> Query()
    {
        return ctx.Configs
            .AsQueryable();
    }

    public async Task<MatchConfig?> ByName(string name)
    {
        return await Query()
            .Where(c => c.Name == name)
            .SingleOrDefaultAsync();
    }
}