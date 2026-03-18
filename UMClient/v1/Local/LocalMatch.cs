using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UMCore.Matches;
using UMCore.Matches.Players.Controllers;

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
	[Export]
	public AcceptDialog CrashedMatchDialogNode { get; set; }
	[Export]
	public TextEdit CrashedMatchExceptionTextNode { get; set; }
	[Export]
	public AcceptDialog FinishedMatchDialogNode { get; set; }

	#endregion

	public Match Match { get; private set; }
	private LocalMatchIOHandler _handler;

	public void Start(Match match, List<PlayerEditorResult> pers, LocalMatchIOHandler handler)
	{
		_handler = handler;
		Match = match;

		OverlayNode.Hide();

		Task.Run(async () => StartMatch(pers));
	}

	private async Task StartMatch(List<PlayerEditorResult> pers)
	{
		try
		{
			var players = new QueuedPlayerCollection(Match.Config);
			Dictionary<string, IPlayerController> controllers = [];
			foreach (var per in pers)
			{
				players.AddPlayer(per.Name, per.TeamIdx, per.Loadout);
				controllers.Add(per.Name, per.Controller);
				// var added = await Match.AddPlayer(
				// 	per.Name,
				// 	per.TeamIdx,
				// 	per.Loadout,
				// 	per.Controller
				// );
				// if (!added)
				// {
				// 	throw new Exception("Failed to add a player, not enough checks");
				// }

				if (per.Textures is null) continue;

				MatchNode.Call("remember_deck_card_back", per.Loadout.Name, per.Textures.Value.CardBack);
				MatchNode.Call("remember_deck_card_images", per.Textures.Value.Cards);
				MatchNode.Call("remember_deck_fighter_images", per.Textures.Value.Fighters);
			}

			var cantRunReason = players.CanRun();
			if (!string.IsNullOrEmpty(cantRunReason))
			{
				throw new Exception($"Failed to add a player, not enough checks: {cantRunReason}");
			}
			await Match.AddPlayers(players, controllers);

			await Match.Run();

			var dialogText = $"Match finished!\nWinning team: {string.Join(", ", Match.GetWinners().Select(p => p.Name))}";
			FinishedMatchDialogNode.DialogText = dialogText;
			FinishedMatchDialogNode.Show();
		} catch (Exception e)
		{
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

			var text = $"Match crashed! Exceptions raised:\n{e.Message}\n{e.StackTrace}";
			if (e.InnerException is not null)
			{
				text = $"{text}\n-===============-{e.InnerException.Message}\n{e.InnerException.StackTrace}\n";
			}
			CrashedMatchExceptionTextNode.Text = text;
			CrashedMatchDialogNode.Show();
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

	public void OnCrashedMatchDialogConfirmed()
	{
		QueueFree();
	}

	public void OnFinishedMatchDialogConfirmed()
	{
		QueueFree();
	}

	public void OnFinishedMatchDialogCanceled()
	{
		QueueFree();
	}

	public void OnCrashedMatchDialogCanceled()
	{
		QueueFree();
	}

	#endregion
}
