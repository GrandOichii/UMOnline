using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface ICoreScriptRepository
{
    IQueryable<CoreScript> Query();
    Task<CoreScript> Active();

}

public class CoreScriptRepository(UMContext ctx) : ICoreScriptRepository
{
    public IQueryable<CoreScript> Query()
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