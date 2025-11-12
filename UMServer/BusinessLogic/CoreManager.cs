using Microsoft.EntityFrameworkCore;
using UMCore.Templates;
using UMDto;
using UMServer.Extensions;
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
        var query = repo.Query();
        var result = await query.SingleAsync(s => s.IsActive);
        return result.Script;
    }
}