using Microsoft.EntityFrameworkCore;
using UMCore.Templates;
using UMServer.Repositories;
using UMDTO;
using UMModel.Models;

namespace UMServer.BusinessLogic;

public interface IUpdateManager
{
    Task<string> Current();
    Task<bool> IsOutdated(DateTime dt);
}

public class UpdateManager(
    IContentUpdateRepository cuRepo
) : IUpdateManager
{
    private async Task<ContentUpdate> GetCurrent() => await cuRepo.Query().Where(cu => cu.IsActive).SingleAsync();

    public async Task<string> Current()
    {
        var current = await GetCurrent();

        return current.Data;
    }

    public async Task<bool> IsOutdated(DateTime dt)
    {
        var current = await GetCurrent();
        return current.CreatedDT > dt;
    }
}