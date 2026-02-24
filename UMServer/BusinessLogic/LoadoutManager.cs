using Microsoft.EntityFrameworkCore;
using UMCore.Templates;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface ILoadoutManager
{
    Task<IEnumerable<LoadoutTemplate>> AllLoadouts();
}

public class LoadoutManager(ILoadoutRepository repo) : ILoadoutManager
{
    public async Task<IEnumerable<LoadoutTemplate>> AllLoadouts()
    {
        return await repo
            .Query()
            .Where(l => l.IsPublic)
            .Select(r => r.ToTemplate())
            .ToListAsync();
    }
}