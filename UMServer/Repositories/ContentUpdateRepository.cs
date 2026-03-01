using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface IContentUpdateRepository
{
    // public IQueryable<ContentUpdate> Query();
    Task<ContentUpdate?> Active();
}

public class ContentUpdateRepository(UMContext ctx) : IContentUpdateRepository
{
    private IQueryable<ContentUpdate> Query()
    {
        return ctx.ContentUpdates
            .AsQueryable();
    }

    public async Task<ContentUpdate?> Active()
    {
        return await Query()
            .Where(c => c.IsActive)
            .SingleOrDefaultAsync();
    }
}