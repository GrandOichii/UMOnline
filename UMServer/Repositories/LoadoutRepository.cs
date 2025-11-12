using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface ILoadoutRepository
{
    public IQueryable<Loadout> Query();
}

public class LoadoutRepository(UMContext ctx) : ILoadoutRepository
{
    public IQueryable<Loadout> Query()
    {
        return ctx.Loadouts
            .Include(l => l.Fighters)
            .Include(l => l.Deck)
            .AsQueryable();
    }
}