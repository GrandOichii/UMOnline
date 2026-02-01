using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMServer.BusinessLogic;

namespace UMServer.Controllers;

[ApiController]
[Route("/api/v1/Update")]
public class UpdateController(IUpdateManager updateManager) : ControllerBase
{
    /// <summary>
    /// Get latest versions of all loadouts, fighters, cards and the core script
    /// </summary>
    /// <returns></returns>
    [HttpGet("Current")]
    public async Task<IActionResult> GetCurrent()
    {
        return Ok(await updateManager.Current());
    }

    [HttpPost("IsOutdated")]
    public async Task<IActionResult> CheckIsOutdated([FromBody] DateTime dt)
    {
        return Ok(await updateManager.IsOutdated(dt));
    }
}