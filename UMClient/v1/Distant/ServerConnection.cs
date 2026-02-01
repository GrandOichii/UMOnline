using Godot;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using UMDTO;

public partial class ServerConnection : Node
{
    [Export]
    public string DefaultAddress { get; set; }

    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public HttpRequest UpdateContentRequestNode { get; set; }

    #endregion

    public override void _Ready()
    {
        // var err = UpdateContentRequestNode.Request($"{DefaultAddress}/api/v1/Update/Current");
    }

    #region Signal connections

    public void OnUpdateContentRequestRequestCompleted(int result, int responseCode, string[] headers, byte[] body)
    {
        // TODO check result and responseCode
        // GD.Print(Encoding.UTF8.GetString(body));
        var content = JsonSerializer.Deserialize<ContentUpdateGet>(body, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        })
            ?? throw new Exception("Failed to deserialize content update from server");

        // GD.Print(content.Core);
        // TODO
    }

    #endregion

}
