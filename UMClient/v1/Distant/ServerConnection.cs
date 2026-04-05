using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UMCore.Templates;
using UMDTO;
using UMModel.Models;

public partial class ServerConnection : Node
{
    #region Signals

    [Signal]
    public delegate void ContentUpdateFinishedEventHandler();
    [Signal]
    public delegate void ContentUpdateFailedEventHandler(string errMsg);
    [Signal]
    public delegate void ContentOutdatedRespondedEventHandler(bool isOutdated);

    #endregion


    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public HttpRequest UpdateContentRequestNode { get; set; }
    [Export]
    public HttpRequest OutdatedContentRequestNode { get; set; }

    #endregion

    private string _address;
    public void SetAddress(string address)
    {
        _address = address;
    }

    private HubConnection _connection = null;
    
    public override void _Notification(int what)
    {
        if (what != NotificationWMCloseRequest) return;
        if (_connection is null) return;
        
        _connection.StopAsync().Wait();
    }

    public void ListenForChatUpdates(Action<ChatMessage> onChatUpdate)
    {
        _connection.On("ChatUpdate", onChatUpdate);
    }

    public async Task<string> Connect(
        string address,
        string name,
        Action<List<MatchProcessGet>> onUpdateTables
    )
    {
        _address = address;

        try
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{address}/Matches")
                .Build();

            _connection.On("UpdateTables", onUpdateTables);

            await _connection.StartAsync();

            var registrationError = await _connection.InvokeAsync<string>("RegisterName", name);
            if (!string.IsNullOrEmpty(registrationError))
            {
                return registrationError;
            }
            return "";
        } catch (Exception e)
        {
            // TODO
            return e.Message;
        }
    }

    public void RequestIsOutdated(DateTime dt)
    {
        var err = OutdatedContentRequestNode.Request(
            $"{_address}/api/v1/Update/IsOutdated",
            [
                "Content-Type: application/json",
            ],
            Godot.HttpClient.Method.Post,
            JsonSerializer.Serialize(dt)
        );
        if (err == Error.Ok)
        {
            return;
        }

        // TODO handle better
        GD.Print($"{nameof(RequestIsOutdated)}: {err}");
    }

    public async Task<string> CreateMatch(CreateMatchParams create)
    {
        return await _connection.InvokeAsync<string>("CreateMatch", create);
    }

    public async Task<string> ConnectToMatch(string matchId)
    {
        return await _connection.InvokeAsync<string>("Connect", matchId);
    }

    public async Task PublishToChat(string matchId, string msg)
    {
        await _connection.SendAsync("PublishToChat", matchId, msg);
    }

    public async Task ForceTableUpdate()
    {
        await _connection.SendAsync("UpdateMe");
    }

    public async Task SelectTeam(string matchId, int teamIdx)
    {
        var err = await _connection.InvokeAsync<string>("SelectTeam", matchId, teamIdx);
        if (err == string.Empty) return;

        throw new Exception($"Failed to select team: {err}");
    }
    
    public async Task SelectLoadout(string matchId, string loadoutName)
    {
        var err = await _connection.InvokeAsync<string>("SelectLoadout", matchId, loadoutName);
        if (err == string.Empty) return;

        throw new Exception($"Failed to select loadout: {err}");
    }

    public async Task<MatchRecordGet> GetRecord(string matchId)
    {
        var client = new System.Net.Http.HttpClient()
        {
            BaseAddress = new(_address)
        };

        var result = await client.GetFromJsonAsync<MatchRecordGet>($"api/v1/Matches/Record/{matchId}");
        return result;
    }

    public async Task StartMatch(string matchId)
    {
        await _connection.SendAsync("Start", matchId);
    }

    public void RequestContentSynchronization()
    {
        var err = UpdateContentRequestNode.Request($"{_address}/api/v1/Update/Current");
        if (err == Error.Ok)
        {
            return;
        }
        GD.Print($"{nameof(RequestContentSynchronization)}: {err}");
    }

    private ContentUpdateGet _cu;
    public ContentUpdateGet PopContentUpdate()
    {
        var result = _cu;
        _cu = null;
        return result;
    }

    #region HTTP Requests

    public async Task<List<MatchConfig>> FetchConfigs()
    {
        var client = new System.Net.Http.HttpClient()
        {
            BaseAddress = new(_address)
        };

        var configs = await client.GetFromJsonAsync<List<MatchConfig>>("api/v1/Configs/All");
        return configs;
    }

    public async Task<List<LoadoutTemplate>> FetchLoadouts()
    {
        var client = new System.Net.Http.HttpClient()
        {
            BaseAddress = new(_address)
        };

        var loadouts = await client.GetFromJsonAsync<List<LoadoutTemplate>>("api/v1/Loadouts/All");
        return loadouts;
    }

    public async Task<ClientWebSocket> WSConnect(string connectStr)
    {
        var result = new ClientWebSocket();
        var address = _address.Replace("http://", "ws://");
        await result.ConnectAsync(new($"{address}/api/v1/Matches/Connect?connectStr={connectStr}"), CancellationToken.None);
        return result;
    }

    #endregion

    // TODO add handler for CantStart

    #region Signal connections

    public void OnUpdateContentRequestRequestCompleted(HttpRequest.Result result, int responseCode, string[] headers, byte[] body)
    {
        // TODO add more detailed checks
        if (result != HttpRequest.Result.Success)
        {
            EmitSignalContentUpdateFailed("Failed to update content");
            return;
        }

        var content = JsonSerializer.Deserialize<ContentUpdateGet>(body, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new Exception("Failed to deserialize content update from server");

        _cu = content;
        EmitSignalContentUpdateFinished();
    }

    public void OnOutdatedContentRequestRequestCompleted(HttpRequest.Result result, int responseCode, string[] headers, byte[] body)
    {
        if (result != HttpRequest.Result.Success)
        {
            throw new Exception($"Unrecognized response code from server: {responseCode}");
        }

        var isOutdated = JsonSerializer.Deserialize<bool>(body, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        EmitSignalContentOutdatedResponded(isOutdated);
    }

    #endregion

}
