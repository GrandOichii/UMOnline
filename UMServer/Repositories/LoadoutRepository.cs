using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface ILoadoutRepository
{
    public Task<IEnumerable<Loadout>> AllPublic();
    Task<Loadout?> ByName(string name);
}

public class LoadoutRepository(UMContext ctx) : ILoadoutRepository
{
    private IQueryable<Loadout> Query()
    {
        return ctx.Loadouts
            .Include(l => l.Fighters)
            .Include(l => l.Deck);
    }

    public async Task<IEnumerable<Loadout>> AllPublic()
    {
        return Query()
            .Where(l => l.IsPublic);
    }

    public Task<Loadout?> ByName(string name)
    {
        return Query()
            .Where(l => l.Name == name)
            .SingleOrDefaultAsync();
    }
}