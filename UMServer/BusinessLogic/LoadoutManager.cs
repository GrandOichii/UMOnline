using Microsoft.EntityFrameworkCore;
using UMCore.Templates;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface ILoadoutManager
{
    Task<IEnumerable<LoadoutTemplate>> All();
}

public class LoadoutManager(ILoadoutRepository repo) : ILoadoutManager
{
    public async Task<IEnumerable<LoadoutTemplate>> All()
    {
        var loadouts = await repo.AllPublic();
        return loadouts
            .Select(r => r.ToTemplate());
    }
}