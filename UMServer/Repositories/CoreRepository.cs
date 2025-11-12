using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface ICoreScriptRepository
{
    public IQueryable<CoreScript> Query();
}

public class CoreScriptRepository(UMContext ctx) : ICoreScriptRepository
{
    public IQueryable<CoreScript> Query()
    {
        return ctx.CoreScripts
            .AsQueryable();
    }

}