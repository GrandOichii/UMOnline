using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMServer.BusinessLogic;

namespace UMServer.Controllers;

[ApiController]
[Route("/api/v1/Matches")]
public class MatchesController(IMatchesManager matchesManager) : ControllerBase
{
    [HttpGet("Create")]
    public async Task CreateMatch()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest) {
            // var userId = this.ExtractClaim(ClaimTypes.NameIdentifier);
            // var userId = "";

            try {
                await matchesManager.WebSocketCreate(HttpContext.WebSockets);
            } catch (Exception e) {
                // TODO handle
                System.Console.WriteLine(e);
                throw;
            }

            // try {
            //     await _matchService.WSConnect(HttpContext.WebSockets, userId, matchId);
            // } catch (InvalidMatchIdException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest; 
            // } catch (MatchNotFoundException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            // } catch (MatchRefusedConnectionException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            // }
        } else {
            HttpContext.Response.StatusCode = 400;
        }
    }
}