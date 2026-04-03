using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UMDTO;
using UMServer.BusinessLogic;
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
        var data = """
        {
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
        "seed": 2050424881,
        "players": [
            {
            "name": "Client2",
            "teamIdx": 0,
            "loadout": "Medusa",
            "responses": {
                "actions": [
                "Scheme",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Scheme",
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
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
                "Attack",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Scheme",
                "Manoeuvre"
                ],
                "attackChoices": [
                "2_5_24",
                "0_5_9",
                "3_4_18",
                "3_5_6",
                "2_4_5",
                "3_4_8"
                ],
                "cardChoices": [
                "28",
                "29",
                "1"
                ],
                "cardOrNothingChoices": [
                "2",
                "",
                "22",
                "11",
                "23",
                "27",
                "21",
                "12",
                "19",
                "17",
                "",
                "",
                "16",
                "15",
                "",
                "3",
                "7",
                "14",
                "25",
                "13",
                "20",
                "26",
                "4",
                "10",
                "0",
                ""
                ],
                "fighterChoices": [
                "2",
                "3",
                "2",
                "0",
                "0",
                "3",
                "1",
                "2",
                "3",
                "2",
                "0",
                "2",
                "0",
                "3",
                "0",
                "1",
                "3",
                "0",
                "2",
                "0",
                "3",
                "1",
                "3",
                "0",
                "1",
                "2",
                "3",
                "2",
                "0",
                "1",
                "2",
                "3",
                "1",
                "0",
                "1",
                "3",
                "0",
                "2",
                "0",
                "1",
                "3",
                "2",
                "2",
                "0",
                "1",
                "3",
                "3",
                "2",
                "1",
                "2",
                "3",
                "1",
                "3",
                "2",
                "1",
                "1",
                "3",
                "2",
                "2",
                "3",
                "1",
                "2",
                "1",
                "3",
                "3",
                "2",
                "2",
                "3",
                "3",
                "2",
                "2",
                "3",
                "2",
                "3",
                "2",
                "3",
                "2",
                "3"
                ],
                "nodeChoices": [
                "15",
                "22",
                "14",
                "20"
                ],
                "pathChoices": [
                "16_13_16",
                "14_13_14_13",
                "15_16_15_14",
                "22_23_22_23",
                "14_15_14_15_22_21",
                "16_13_17_27_26",
                "23_21_20_13_9",
                "13_20_24_20_21_23",
                "26_27_17_27_28_29_18",
                "23_22_21_22_21_13_9",
                "21_22_21_13_20_13",
                "9_13_17_12_27_0_27_17",
                "13_17_13_21_23_22_15_16",
                "18_29_28_27_28_27_0_27",
                "27_17_13_21",
                "16_13_9_13",
                "17_13_21_23",
                "23_21_13_17",
                "13_20_13_21_23",
                "21_20_13_16_15",
                "20_19_24_19_25",
                "15_22_23_21_22_15_14",
                "23_21_20_13_17_18_29",
                "25_24_19_24_19_20_21",
                "17_13_17_12_10_11_9",
                "14_13_9_13_9_8_7",
                "9_13_14_13_20_21_22",
                "29_28_30_28_29",
                "21_22_21_20_13_14",
                "22_21_20_19_20_13_17_0",
                "7_9_13_9_7_6_8_7",
                "14_15_14_13_9_8_6_4",
                "29_18_17_0_17_18_19_20",
                "4_1_4_6_4",
                "7_6_8_6_7_8",
                "20_13_9_13_21_22",
                "0_27_28_27_28_29",
                "22_15_22_21_22",
                "4_6_8_9_8_9",
                "8_7_8_7_6_8",
                "29_18_19_20_24_19",
                "19_24_20_24",
                "22_15",
                "9_10_9",
                "8_7_6",
                "6_7_8_9_13_9_8",
                "24_19_18_29_18_19_20",
                "9_11_9_7_9_10_9",
                "20_24_19_24_20_21_23",
                "8_7_8_9_8_6_7",
                "9_10_9_13_17",
                "7_9_7",
                "23_21_13_16",
                "17_13_16_13",
                "13_21_23_21_23",
                "7_6_4_6_8_9",
                "16_13_17_0_17_12",
                "9_8_6",
                "12_17_0_27",
                "6_8_7_6_8",
                "23_22_23_22",
                "27_0_2_0_17_0_27",
                "22_15_16_15_16_15_16",
                "8_9_8_6",
                "6_7_6_7_6_7_6",
                "27_28_27_28_27_17_27_17",
                "17_27_17_27",
                "27_17_27_12_0",
                "6_8_9_7_8_7_9",
                "9_11_9_10_12_0_12",
                "0_12_10_12_27_12_0",
                "0_27_12_0_12_17",
                "12_27_26_25",
                "17_0_12_10_9_8_7_9",
                "25_19_18_19_24_19_25",
                "9_10_12_17",
                "25_19_25_19",
                "19_25_19"
                ],
                "playerChoices": [],
                "stringChoices": [
                "No"
                ],
                "tokenChoices": []
            }
            },
            {
            "name": "Client1",
            "teamIdx": 1,
            "loadout": "Sinbad",
            "responses": {
                "actions": [
                "Scheme",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
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
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Attack",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre",
                "Manoeuvre"
                ],
                "attackChoices": [
                "5_1_42",
                "5_0_45",
                "4_1_59",
                "4_3_39"
                ],
                "cardChoices": [
                "48",
                "26"
                ],
                "cardOrNothingChoices": [
                "47",
                "32",
                "58",
                "44",
                "",
                "36",
                "53",
                "46",
                "57",
                "37",
                "33",
                "",
                "49",
                "43",
                "34",
                "55",
                "31",
                "",
                "35",
                "54",
                "50",
                "41",
                "40",
                "56",
                "51",
                "38",
                "30",
                "52"
                ],
                "fighterChoices": [
                "4",
                "5",
                "4",
                "5",
                "4",
                "5",
                "5",
                "4",
                "4",
                "5",
                "4",
                "5",
                "5",
                "4",
                "4",
                "5",
                "5",
                "4",
                "5",
                "4",
                "4",
                "5",
                "5",
                "4",
                "4",
                "5",
                "5",
                "4",
                "4",
                "5",
                "5",
                "4",
                "4",
                "5",
                "4",
                "5",
                "2",
                "5",
                "4",
                "4",
                "4",
                "4",
                "4",
                "4",
                "4"
                ],
                "nodeChoices": [
                "7"
                ],
                "pathChoices": [
                "4_6_8_6",
                "7_8_7_8",
                "6_8_6_7",
                "8_6_7_6",
                "7_9_11_9",
                "6_8_6_8",
                "8_6_8_7_8",
                "9_10_12_0_27",
                "27_17_27_17_13",
                "8_6_7_6_8",
                "13_20_24_25_26_27",
                "8_6_7_6_4_5",
                "5_3_2_30",
                "27_12_17_18_17_18",
                "18_19_18_29_18_17",
                "30_28_30_31_5_31",
                "31_5_4_6_4_1",
                "17_12_27_28_30_28_29",
                "1_4_1_0_17_13_21_23",
                "29_28_27_17_0_17_12_27",
                "27_17_27_17_27_17_27",
                "23_21_22_21_20_13_14",
                "14_13_9_13_16_13_16",
                "27_28_30_2_0_12_27",
                "27_0_27_0_12_0_27",
                "16_13_20_13_21_20",
                "20_24_20_13_20_19",
                "27_0_17_27_26_27_17",
                "17_27_26_25_26_27_0_17",
                "19_20_19_20_21_20_21_13",
                "13_14_13_17_27_0_1",
                "17_13_9_13_20_19_20",
                "20_13_17_13_14_13_20_13_14",
                "1_0_27_28_27_17_13_20_21",
                "14_13_20_13_20_13_20_21_13",
                "21_13_21_13_17_0_1_4_1",
                "17_0_12_27_17",
                "1_0_12_17_13_21_13_14_15_22",
                "13_14_15_22_21_13_16_13_21_23",
                "23_22_15_14_13_21_22_23_21_20_13",
                "13_9_11_9_13_16_15_22_15_16_15",
                "15_14_13_17_18_19_25_24_19_20_24",
                "24_19_20_24_19_20_13_14_13",
                "13_9_13_9_10_11_9_7",
                "7_6_7_6_8_6_7_9_8"
                ],
                "playerChoices": [],
                "stringChoices": [],
                "tokenChoices": []
            }
            }
        ]
        }
        """;
        return Ok(JsonSerializer.Deserialize<MatchRecordGet>(data, JsonSerializerOptions.Web));

        // try
        // {
        //     var record = await matchesManager.GetRecord(matchId);
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