using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UMDTO;
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
    // TODO remove
    [HttpGet("All")]
    public async Task<IActionResult> All()
    {
        return Ok(await matchesManager.All());
    }

    [HttpGet("Record/{matchId}")]
    public async Task<IActionResult> GetRecord(string matchId)
    {
        var json = """
{
    "result": {
        "config": {
        "randomMatch": true,
        "seed": 0,
        "initialHandSize": 5,
        "actionsPerTurn": 2,
        "maxHandSize": 7,
        "manoeuvreDrawAmount": 1,
        "randomFirstPlayer": true,
        "firstPlayerIdx": -1,
        "exhaustDamage": 2,
        "teamSize": 1,
        "teamCount": 2
        },
        "seed": 1537979044,
        "players": [
        {
            "name": "Client2",
            "teamIdx": 0,
            "loadout": "Alice",
            "responses": {
            "actions": [
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Scheme",
                "Attack",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Scheme",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Scheme",
                "Manoeuvre",
                "Attack",
                "Scheme",
                "Manoeuvre",
                "Manoeuvre"
            ],
            "attackChoices": [
                "0_3_23",
                "0_3_21",
                "0_2_22"
            ],
            "cardChoices": [
                "27",
                "25",
                "",
                "18",
                "26",
                "14",
                "",
                "",
                "24",
                "19",
                "9",
                "5",
                "28",
                "10",
                "12",
                "17",
                "16",
                "7",
                "3",
                "1",
                "20",
                "0",
                "13",
                "4",
                "15",
                "8",
                "2",
                "11",
                "6"
            ],
            "cardOrNothingChoices": [],
            "fighterChoices": [
                "0",
                "1",
                "0",
                "1",
                "1",
                "0",
                "1",
                "0",
                "3",
                "0",
                "1",
                "0",
                "1",
                "0",
                "1",
                "0",
                "1",
                "1",
                "0",
                "0",
                "1",
                "0",
                "1",
                "0",
                "1",
                "1",
                "0",
                "1",
                "0",
                "1",
                "1",
                "0",
                "0",
                "1",
                "1",
                "0",
                "0",
                "1",
                "1",
                "0",
                "1",
                "0"
            ],
            "nodeChoices": [
                "15"
            ],
            "pathChoices": [
                "16_13_17_13",
                "15_16_15_16",
                "13_17_0_12_10",
                "16_13_16_15_14",
                "22_15_22",
                "12_0",
                "22_21_13_14_13",
                "0_12_0_17_18",
                "7_6_8",
                "19_20_19_25_24_19_25",
                "9_13_16_13_17_13",
                "25_19",
                "13_17_13",
                "19_25_19",
                "13_16",
                "19_24_19_18_29",
                "16_13_17_0",
                "0_27_28_30_31",
                "29_28_27_0_2",
                "2_3_2",
                "2_3_2_3_2_3_2",
                "31_5_4_1_4_5",
                "2_30_28_27_12_17",
                "5_4_1_4_5_3",
                "17_0_27_17_13",
                "3_2_0_27_28",
                "28_30_28_29",
                "13_21_22_21",
                "29_28",
                "21_13_16",
                "16_13_9",
                "28_27_17_13_14",
                "14_13_9_10_11",
                "9_13_9_13",
                "13_17_12_17",
                "11_9_13_9",
                "9_8_7_9_13",
                "17_13_9_7",
                "7_8_7_6_8",
                "13_20_19_25_26",
                "8_9_10_11",
                "26_27_26_25_19_20_24",
                "11_9_13_17_12_17_18",
                "18_17_13_21_13_21",
                "24_20_13_17_12",
                "21_13_16_13_17"
            ],
            "playerChoices": [],
            "stringChoices": [
                "SMALL"
            ],
            "tokenChoices": []
            }
        },
        {
            "name": "Client21",
            "teamIdx": 1,
            "loadout": "King Arthur",
            "responses": {
            "actions": [
                "Scheme",
                "Manoeuvre",
                "Scheme",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Scheme",
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
                "Attack",
                "Manoeuvre"
            ],
            "attackChoices": [
                "3_1_50",
                "2_1_46",
                "2_0_37",
                "2_0_40",
                "2_0_37"
            ],
            "cardChoices": [
                "33",
                "44",
                "34",
                "48",
                "55",
                "58",
                "32",
                "",
                "56",
                "",
                "",
                "",
                "49",
                "54",
                "57",
                "",
                "",
                "43",
                "51",
                "47",
                "",
                "59",
                "52",
                "",
                "41",
                "",
                "",
                ""
            ],
            "cardOrNothingChoices": [],
            "fighterChoices": [
                "2",
                "1",
                "0",
                "3",
                "2",
                "2",
                "3",
                "1",
                "2",
                "3",
                "3",
                "2",
                "3",
                "2",
                "3",
                "2",
                "3",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2",
                "2"
            ],
            "nodeChoices": [
                "7"
            ],
            "pathChoices": [
                "4_1_0_27",
                "14_15_22",
                "10_12_17_12",
                "7_6_8_6",
                "6_7_9_7_9",
                "27_28_29_28_27",
                "27_0_2_0",
                "9_11_9_7",
                "13_9_8_9",
                "18_19_18_19",
                "0_1_0_2",
                "8_7_9_10",
                "10_9_7_6_7",
                "2_0_17_0_2",
                "7_8_9_7",
                "2_30_28_27_17",
                "7_9",
                "17_13_21",
                "9_13_17_12_0",
                "21_23_22_15_22",
                "22_21_13",
                "13_20_24_20",
                "20_24",
                "24_19_20_24_20",
                "20_21_22",
                "22_23_21_20",
                "20_19_24_25_24",
                "24_19_18",
                "18_29",
                "29_28_30_28",
                "28_29",
                "29_28_27"
            ],
            "playerChoices": [],
            "stringChoices": [
                "Yes"
            ],
            "tokenChoices": []
            }
        }
        ]
    },
    "id": 1,
    "exception": null,
    "status": 5,
    "isCanceled": false,
    "isCompleted": true,
    "isCompletedSuccessfully": true,
    "creationOptions": 0,
    "asyncState": null,
    "isFaulted": false
    }
""";
        return Ok(json);

        // try
        // {
        //     var record = matchesManager.GetRecord(matchId);
        //     return Ok(record);
        // }
        // catch (MatchNotFinishedException e)
        // {
        //     return BadRequest(e.Message);
        // }
        // catch (MatchNotFoundException e)
        // {
        //     return BadRequest(e.Message);
        // }
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