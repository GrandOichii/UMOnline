using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface ICoreScriptRepository
{
    Task<CoreScript> Active();

}

public class CoreScriptRepository(UMContext ctx) : ICoreScriptRepository
{
    private IQueryable<CoreScript> Query()
    {
        return ctx.CoreScripts
            .AsQueryable();
    }

    public async Task<CoreScript> Active()
    {
        return await Query()
            .Where(s => s.IsActive)
            .SingleAsync();
    }

}