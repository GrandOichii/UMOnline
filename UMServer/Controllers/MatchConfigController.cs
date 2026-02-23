using Microsoft.AspNetCore.Mvc;
using UMServer.BusinessLogic;

namespace UMServer.Controllers;

[ApiController]
[Route("/api/v1/Configs")]
public class MatchConfigController(
    IMatchConfigManager configs
) : ControllerBase
{
    [HttpGet("All")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(
            await configs.All()
        );
    }
}