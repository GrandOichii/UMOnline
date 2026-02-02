using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UMCore.Matches;

public partial class LocalMatch : Control
{
	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public ColorRect OverlayNode { get; set; }
    [Export]
    public Control MatchNode { get; set; }
    [Export]
    public Node ConnectionNode { get; set; }

	#endregion

    private Match _match;

    public void Start(Match match, List<PlayerEditorResult> pers)
    {
        _match = match;

        // TODO

        OverlayNode.Hide();

        _ = StartMatch(pers);
    }

    private async Task StartMatch(List<PlayerEditorResult> pers)
    {
        foreach (var per in pers)
		{
			var added = await _match.AddPlayer(
				per.Name,
				per.TeamIdx,
				per.Loadout,
				per.Controller
			);
			if (!added)
			{
				throw new Exception("Failed to add a player, not enough checks");
			}
		}

        await _match.Run();
    }
}
