using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMServer.BusinessLogic;

namespace UMServer.Controllers;

[ApiController]
[Route("/api/v1/CoreScripts")]
public class CoreScriptController(ICoreScriptManager coreScriptManager) : ControllerBase
{
    [HttpGet("Active")]
    public async Task<IActionResult> GetActive()
    {
        return Ok(await coreScriptManager.Active());
    }
}