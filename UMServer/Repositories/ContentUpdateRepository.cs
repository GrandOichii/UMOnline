using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface IContentUpdateRepository
{
    public IQueryable<ContentUpdate> Query();
}

public class ContentUpdateRepository(UMContext ctx) : IContentUpdateRepository
{
    public IQueryable<ContentUpdate> Query()
    {
        return ctx.ContentUpdates
            .AsQueryable();
    }
}