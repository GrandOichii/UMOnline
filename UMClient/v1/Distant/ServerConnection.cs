using Godot;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using UMDTO;

public partial class ServerConnection : Node
{
    #region Signals

    [Signal]
    public delegate void ContentUpdateFinishedEventHandler();
    [Signal]
    public delegate void ContentUpdateFailedEventHandler(string errMsg);

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
        
        // TODO
    }

    public void RequestIsOutdated(DateTime dt)
    {
        GD.Print(_address);
        var err = OutdatedContentRequestNode.Request($"{_address}/api/v1/Update/IsOutdated", null, Godot.HttpClient.Method.Post, JsonSerializer.Serialize(dt));
        GD.Print($"{nameof(RequestIsOutdated)}: {err}");
        // TODO check err
    }

    public void RequestContentSynchronization()
    {
        GD.Print(_address);
        var err = UpdateContentRequestNode.Request($"{_address}/api/v1/Update/Current");
        GD.Print($"{nameof(RequestContentSynchronization)}: {err}");
    }

    public override void _Ready()
    {
    }

    private ContentUpdateGet _cu;
    public ContentUpdateGet PopContentUpdate()
    {
        var result = _cu;
        _cu = null;
        return result;
    }

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
        // TODO
    }

    #endregion

}
