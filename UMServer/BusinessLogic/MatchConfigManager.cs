using Microsoft.EntityFrameworkCore;
using UMModel.Models;
using UMServer.Repositories;

namespace UMServer.BusinessLogic;

public interface IMatchConfigManager
{
    Task<IEnumerable<MatchConfig>> All();
}

public class MatchConfigManager(
    IMatchConfigRepository repo
) : IMatchConfigManager
{
    public async Task<IEnumerable<MatchConfig>> All()
    {
        return await repo.All();
    }
}