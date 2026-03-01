using Microsoft.EntityFrameworkCore;
using UMCore.Templates;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface ICoreScriptManager
{
    Task<string> Active();
}

public class CoreScriptManager(ICoreScriptRepository repo) : ICoreScriptManager
{
    public async Task<string> Active()
    {
        var result = await repo.Active();
        return result.Script;
    }
}