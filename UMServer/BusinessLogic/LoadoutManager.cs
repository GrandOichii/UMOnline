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
        var query = repo.Query();
        var result = await query.ToListAsync();
        return result.Select(r => r.ToTemplate());
    }
}