using Godot;
using System;
using System.Net.WebSockets;
using UMDTO;

public partial class DistantMatchWindow : Window
{
	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public DistantMatch MatchNode { get; set; }

	#endregion

	public string MatchId { get; private set; }

	public void SetEssentials(
		bool clientIsOwner,
		ServerConnection connection,
		ClientWebSocket socket,
		string matchId
	)
	{
		MatchId = matchId;

		Hide();
		ForceNative = true;
		Title = $"Match {matchId}";
		GD.Print(matchId);
		Show();
		MatchNode.SetEssentials(
			clientIsOwner,
			connection,
			socket,
			matchId
		);
	}

	public void Update(MatchProcessGet match)
	{
		MatchNode.Update(match);
	}

	#region Signal connections

	public void OnCloseRequested()
	{
		QueueFree();
	}

	#endregion
}
