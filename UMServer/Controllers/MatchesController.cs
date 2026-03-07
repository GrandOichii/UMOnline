using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMModel;
using UMServer.BusinessLogic;
using UMServer.Extensions;
using UMServer.Services;

namespace UMServer.Controllers;

[ApiController]
[Route("/api/v1/Matches")]
public class MatchesController(
    IMatchManager matchesManager,
    IMatchConnectEndpointSerializer connectSerializer
) : ControllerBase
{
    [HttpGet("Record/{matchId}")]
    public async Task<IActionResult> GetRecord(string matchId)
    {
        try
        {
            var record = matchesManager.GetRecord(matchId);
            return Ok(record);
        }
        catch (MatchNotFinishedException e)
        {
            return BadRequest(e.Message);
        }
        catch (MatchNotFoundException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("Connect")]
    public async Task Connect([FromQuery] string connectStr)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            var (connectionId, matchId) = connectSerializer.Deserialize(connectStr);
            if (string.IsNullOrEmpty(connectionId))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            await matchesManager.WSTryConnect(
                HttpContext.WebSockets,
                connectionId,
                matchId
            );
            // await socket.CloseAsync(System.Net.WebSockets.WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);

            // try {
            //     await matchesManager.WebSocketCreate(HttpContext.WebSockets);
            // } catch (Exception e) {
            //     // TODO handle
            //     System.Console.WriteLine(e);
            //     throw;
            // }

            // try {
            //     await _matchService.WSConnect(HttpContext.WebSockets, userId, matchId);
            // } catch (InvalidMatchIdException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest; 
            // } catch (MatchNotFoundException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            // } catch (MatchRefusedConnectionException) {
            //     HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            // }
        }
        else
        {
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}