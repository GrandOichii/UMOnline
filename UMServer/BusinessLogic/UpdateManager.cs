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

    public async Task<string> Current()
    {
        var current = await cuRepo.Active();

        return current?.Data ?? string.Empty;
    }

    public async Task<bool> IsOutdated(DateTime dt)
    {
        var current = await cuRepo.Active();
        if (current is null) return false;

        return current.CreatedDT > dt;
    }
}