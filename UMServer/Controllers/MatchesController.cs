using System.Net;
using System.Text.Json;
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
        return Ok(new MatchRecordGet()
        {
            Seed = 436697265,
            Config = new()
            {
                RandomMatch = true,
                Seed = 0,
                InitialHandSize = 5,
                ActionsPerTurn = 2,
                MaxHandSize = 7,
                ManoeuvreDrawAmount = 1,
                RandomFirstPlayer = true,
                FirstPlayerIdx = -1,
                ExhaustDamage = 2,
                TeamSize = 1,
                TeamCount = 2
            },
            Players = [
                new() {
                    Name = "Client21",
                    TeamIdx = 0,
                    Loadout = "Alice",
                    Responses = new() {
                        Actions = [
                            "Scheme",
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
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Attack",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Scheme",
                            "Manoeuvre",
                            "Attack",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Attack",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre"
                        ],
                        AttackChoices = [
                            "1_3_36",
                            "0_3_41",
                            "1_2_55",
                            "0_3_46"
                        ],
                        CardChoices = [],
                        CardOrNothingChoices = [
                            "34",
                            "39",
                            "30",
                            "",
                            "50",
                            "43",
                            "44",
                            "53",
                            "56",
                            "38",
                            "48",
                            "49",
                            "33",
                            "45",
                            "59",
                            "37",
                            "",
                            "",
                            "31",
                            "",
                            "40",
                            "",
                            "32",
                            "47",
                            "51",
                            "57",
                            "58",
                            "42",
                            "54",
                            "",
                            "52"
                        ],
                        FighterChoices = [
                            "0",
                            "1",
                            "0",
                            "1",
                            "1",
                            "0",
                            "1",
                            "0",
                            "1",
                            "0",
                            "1",
                            "0",
                            "0",
                            "1",
                            "1",
                            "0",
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
                            "0",
                            "1",
                            "1",
                            "0",
                            "0",
                            "1",
                            "0",
                            "0",
                            "0"
                        ],
                        NodeChoices = [
                            "7"
                        ],
                        PathChoices = [
                            "4_1_0_27",
                            "27_17_27_12_27",
                            "7_8_7_9_8_6",
                            "27_17_27_12_27",
                            "6_8_9_13",
                            "13_9_7_6",
                            "27_12_10",
                            "6_4_1_4_1",
                            "10_11_9_10",
                            "1_0_27_17_0",
                            "10_11_9_7_9_7_6",
                            "0_17_27_12_10",
                            "6_4_6_8_6",
                            "6_7_6_8",
                            "10_9_10_9",
                            "9_8_6_8_9",
                            "8_6_7_9_8",
                            "9_7_9_10_9",
                            "8_6_7_9_11",
                            "11_10_11_9_8_6_8",
                            "9_7_9_11_9_7_6",
                            "6_8_7_9_13_21_22",
                            "8_7_8_9_7_9_10",
                            "10_12_27_28_30",
                            "22_23_22_21_23",
                            "23_22",
                            "30_2",
                            "2_30_2_0_1",
                            "1_0_12_0_27",
                            "22_21_22_21_13",
                            "13_21_13",
                            "27_17_12",
                            "12_27_12_17_27",
                            "13_9_10_12_17_18",
                            "18_29_28",
                            "27_28_30",
                            "1_0_12_17",
                            "26_27_28",
                            "17_0_17_12_27",
                            "28_29_18_17_27_0_2",
                            "2_0_27_17_13",
                            "27_17_13_17_18",
                            "18_19_24_25_24",
                            "24_25_19",
                            "19_20_19",
                            "19_20_19_20_19_24_19",
                            "19_18_17"
                        ],
                        PlayerChoices = [],
                        StringChoices = [
                            "SMALL"
                        ],
                        TokenChoices = []
                    }
                },
                new() {
                    Name = "Client211",
                    TeamIdx = 1,
                    Loadout = "King Arthur",
                    Responses = new() {
                        Actions = [
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Attack",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Scheme",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Attack",
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
                            "Scheme",
                            "Attack",
                            "Attack",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre",
                            "Manoeuvre"
                        ],
                        AttackChoices = [
                            "3_0_26",
                            "3_1_13",
                            "3_1_10",
                            "2_1_14",
                            "3_1_20",
                            "2_1_7"
                        ],
                        CardChoices = [],
                        CardOrNothingChoices = [
                            "24",
                            "25",
                            "29",
                            "15",
                            "22",
                            "",
                            "",
                            "17",
                            "8",
                            "5",
                            "19",
                            "",
                            "",
                            "27",
                            "2",
                            "1",
                            "0",
                            "",
                            "12",
                            "",
                            "4",
                            "",
                            "28",
                            "23",
                            "21",
                            "",
                            "",
                            "9",
                            "3",
                            "16",
                            "18"
                        ],
                        FighterChoices = [
                            "3",
                            "2",
                            "2",
                            "3",
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
                            "3",
                            "2",
                            "3",
                            "3",
                            "2",
                            "3",
                            "2",
                            "3",
                            "2",
                            "2",
                            "3",
                            "3",
                            "2",
                            "2",
                            "3",
                            "3",
                            "2",
                            "0",
                            "1",
                            "3",
                            "3",
                            "2",
                            "2",
                            "3",
                            "3",
                            "2",
                            "3",
                            "2",
                            "2",
                            "2",
                            "2",
                            "2"
                        ],
                        NodeChoices = [
                            "15"
                        ],
                        PathChoices = [
                            "15_14_13_21",
                            "16_13_20",
                            "20_13_21_20_19",
                            "21_23_22_15",
                            "19_25_19_20_13",
                            "15_22_21_13_16",
                            "16_13_20_24_20",
                            "13_21_22_23_22_15",
                            "20_19_18_19_18",
                            "15_14_15",
                            "18_19_18",
                            "15_14_15",
                            "18_19",
                            "15_14_13_20",
                            "19_18_19_18",
                            "20_24_19_25_26",
                            "18_19_18_17_13",
                            "26_25_19_24",
                            "13_17_27_17",
                            "17_0_2",
                            "24_20",
                            "2_3",
                            "20_24_25",
                            "3_5_3",
                            "25_19_24_20_21",
                            "21_13_17_18_29",
                            "3_5_31_5",
                            "5_31_5",
                            "29_28_30",
                            "30_2_30_31",
                            "5_4_1",
                            "1_4_6",
                            "31",
                            "30_2_0_1",
                            "28_27_26",
                            "6_4_5",
                            "31_30_28_30",
                            "5_4_1_0",
                            "30_31",
                            "31_5_31",
                            "0_17_12_10",
                            "10_12_17_0_17",
                            "31_5_4",
                            "17_13_20",
                            "4_5_31",
                            "31_5_4_1",
                            "1_0_17_12_27",
                            "27_0_12_0_27_12",
                            "12_10_12_27"
                        ],
                        PlayerChoices = [],
                        StringChoices = [
                            "{King Arthur_Noble Sacrifice$16$Noble Sacrifice}",
                            "{King Arthur_Regroup$18$Regroup}",
                            "{King Arthur_Feint$10$Feint}",
                            "No",
                            "Yes"
                        ],
                        TokenChoices = []
                    }
                }
            ]
        });

        
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