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
    private LocalMatchIOHandler _handler;

    public void Start(Match match, List<PlayerEditorResult> pers, LocalMatchIOHandler handler)
    {
        _handler = handler;
        _match = match;

        // TODO

        OverlayNode.Hide();

        _ = StartMatch(pers);
    }

    private async Task StartMatch(List<PlayerEditorResult> pers)
    {
        try
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
        } catch (Exception e)
        {
            // TODO better error handling
            GD.PushError(e);
			GD.Print(e.Message);
			GD.Print(e.StackTrace);
			GD.Print("");
			GD.Print("");
			GD.Print("---====================----");
			GD.Print("");
			GD.Print("");
			GD.Print(e.InnerException?.Message);
			GD.Print(e.InnerException?.StackTrace);
        }
    }

    public void Load(Godot.Collections.Dictionary data)
	{
        ConnectionNode.EmitSignal("match_info_updated", data);
	}

    #region Signal connections

	public void OnConnectionResponded(string response)
	{
		_handler.SetReadTaskResult(response);
	}

    #endregion
}
