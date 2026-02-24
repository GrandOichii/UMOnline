using Godot;
using Godot.Collections;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UMDTO;

public partial class DistantMatch : Control
{
    [Export]
    public Dictionary<int, Color> TeamColors { get; set; }

    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public Control ConnectionOverlayNode { get; set; }
    [Export]
    public Control MatchNode { get; set; }
    [Export]
    public Chat ChatNode { get; set; }
    [Export]
    public Tree PlayerTableNode { get; set; }
    [Export]
    public OptionButton TeamOptionNode { get; set; }
    [Export]
    public OptionButton DeckOptionNode { get; set; }
    [Export]
    public Button StartMatchButtonNode { get; set; }
    [Export]
	public Node ConnectionNode { get; set; }
    [Export]
    public TabContainer MoveChatToNode { get; set; }

    #endregion

    #region Packed scenes

    // [Export]
    // public PackedScene ConnectedPlayerDisplayScene { get; set; }

    #endregion

    public override void _Ready()
    {
        MatchNode.Hide();
        ConnectionOverlayNode.Show();

        // PlayerTableNode setup
        PlayerTableNode.Columns = 2;
        PlayerTableNode.SetColumnTitle(0, "Name");
        PlayerTableNode.SetColumnTitle(1, "Deck");
    }

    private ServerConnection _connection;
    private string _matchId;
    private ClientWebSocket _socket;

    public void SetEssentials(
        bool clientIsOwner,
        ServerConnection connection,
        ClientWebSocket socket,
        string matchId
    )
    {
        _socket = socket;
        _connection = connection;
        _matchId = matchId;

        ChatNode.SetEssentials(
            connection,
            matchId
        );

        StartMatchButtonNode.Visible = clientIsOwner;

        StartReceiveLoop(socket);
    }

    public void Update(MatchProcessGet match)
    {
        // update teams
        if (TeamOptionNode.ItemCount == 0)
        {
            for (int i = 0; i < match.TeamCount; ++i)
            {
                TeamOptionNode.AddItem(i.ToString());
            }
            TeamOptionNode.Select(-1);
        }

        // update allowed loadouts
        if (DeckOptionNode.ItemCount == 0)
        {
            DeckOptionNode.Clear();
            foreach (var deckName in match.AllowedFighters)
            {
                DeckOptionNode.AddItem(deckName);
            }
            DeckOptionNode.Select(-1);
        }

        // update players
        PlayerTableNode.Clear();
        PlayerTableNode.CreateItem(); // root

        foreach (var player in match.Players)
        {
            var item = PlayerTableNode.CreateItem();
            item.SetCustomColor(0, TeamColors[player.TeamIdx]);
            item.SetText(0, player.Name);
            item.SetText(1, player.LoadoutName ?? "");
        }
    }

    public void StartReceiveLoop(ClientWebSocket socket) {
		Task.Run(async () =>
		{
			while (socket.State == WebSocketState.Open)
			{
				var message = await WSRead(socket);

				// await OnReceive?.Invoke(message);
                Load(Json.ParseString(message));
			}
			// OnClose?.Invoke();
		});
	}

    
    public void Load(Variant data)
	{
        Callable.From(() =>
        {
            if (ConnectionOverlayNode.Visible)
            {
                ConnectionOverlayNode.Hide();
                MatchNode.Show();
                ChatNode.Reparent(MoveChatToNode);
            }
		    ConnectionNode.EmitSignal("match_info_updated", data);
        }).CallDeferred();
	}

    // TODO this shouldnt be here
    private static async Task<string> WSRead(ClientWebSocket socket) {
		WebSocketReceiveResult result;
		var buffer = new ArraySegment<byte>(new byte[1024]);
		var message = new StringBuilder();
		do
		{
			result = await socket.ReceiveAsync(buffer, CancellationToken.None);
			string messagePart = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
			message.Append(messagePart);
		}
		while (!result.EndOfMessage);
		return message.ToString();
	}

    #region Signal connections

    public async void OnConnectionResponded(string resp)
    {
        var serverMsg = Encoding.UTF8.GetBytes(resp);
        await _socket.SendAsync(new ArraySegment<byte>(serverMsg, 0, serverMsg.Length), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async void OnTeamEditItemSelected(int idx)
    {
        var team = int.Parse(TeamOptionNode.GetItemText(idx));
        await _connection.SelectTeam(_matchId, team);
    }

    public async void OnDeckOptionItemSelected(int idx)
    {
        var loadoutName = DeckOptionNode.GetItemText(idx);
        await _connection.SelectLoadout(_matchId, loadoutName);
    }

    public async void OnStartMatchButtonPressed()
    {
        await _connection.StartMatch(_matchId);
    }

    #endregion

}
