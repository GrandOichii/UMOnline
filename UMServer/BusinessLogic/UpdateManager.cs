using Microsoft.EntityFrameworkCore;
using UMCore.Templates;
using UMServer.Repositories;
using UMDTO;

namespace UMServer.BusinessLogic;

public interface IUpdateManager
{
    Task<ContentUpdateGet> Current();
}

public class UpdateManager(
    ICoreScriptRepository coreRepo,
    ILoadoutRepository loadoutRepo
) : IUpdateManager
{
    public async Task<ContentUpdateGet> Current()
    {
        // core
        var query = coreRepo.Query();
        var core = await query.SingleAsync(s => s.IsActive);

        // loadouts
        var loadouts = await loadoutRepo.Query().ToListAsync();

        
        return new()
        {
            Core = core.Script,
            Loadouts = [.. loadouts.Select(l => l.ToTemplate())]
        };
    }
}