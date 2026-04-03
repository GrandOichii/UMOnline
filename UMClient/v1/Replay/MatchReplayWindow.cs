using Godot;
using System;
using System.Threading.Tasks;
using UMCore.Matches;
using UMCore.Matches.Players.Controllers;
using UMCore.Templates;
using UMDTO;

public partial class MatchReplayWindow : Window
{
	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public MatchReplay ReplayNode { get; set; }

	#endregion

	private LocalRepository _repo;
	private MatchRecordGet _record;

	public void LoadMatchRecord(
		LocalRepository repo,
		MatchRecordGet record
	)
	{
		Hide();
		ForceNative = true;
		// TODO set title
		Show();
		ReplayNode.LoadMatchRecord(repo, record);
	}
	
	#region Signal connections
	
	public void OnCloseRequested()
	{
		QueueFree();
	}
	
	#endregion
}
