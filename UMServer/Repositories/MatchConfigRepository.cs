using Microsoft.EntityFrameworkCore;
using UMModel;
using UMModel.Models;

namespace UMServer.Repositories;

public interface IMatchConfigRepository
{
    public IQueryable<MatchConfig> Query();
}

public class MatchConfigRepository(UMContext ctx) : IMatchConfigRepository
{
    public IQueryable<MatchConfig> Query()
    {
        return ctx.Configs
            .AsQueryable();
    }
}