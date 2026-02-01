using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMServer.BusinessLogic;

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