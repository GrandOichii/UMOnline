using Microsoft.AspNetCore.Mvc;

namespace UMServer.Controllers;

[ApiController]
[Route("/api/v1/Home")]
public class HomeController() : ControllerBase
{
    [HttpGet("Ping")]
    public async Task<IActionResult> Ping()
    {
        return Ok();
    }
}