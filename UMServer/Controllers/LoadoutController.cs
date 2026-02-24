using Microsoft.AspNetCore.Mvc;
using UMServer.BusinessLogic;

namespace UMServer.Controllers;

[ApiController]
[Route("/api/v1/Loadouts")]
public class LoadoutController(
    ILoadoutManager loadoutManager
) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(
            await loadoutManager.AllLoadouts()
        );
    }
}