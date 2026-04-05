using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UMCore;
using UMCore.Matches;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Controllers;
using UMCore.Templates;

public partial class LocalMatchesTab : Control
{
	private static readonly Dictionary<string, MatchConfig> PRESETS = new()
	{
		{ "tester", new MatchConfig()
		{
			RandomMatch = false,
			Seed = 0,
			InitialHandSize = MatchConfig.Default1x1.InitialHandSize,
			ActionsPerTurn = MatchConfig.Default1x1.ActionsPerTurn,
			MaxHandSize = MatchConfig.Default1x1.MaxHandSize,
			ManoeuvreDrawAmount = MatchConfig.Default1x1.ManoeuvreDrawAmount,
			RandomFirstPlayer = MatchConfig.Default1x1.RandomFirstPlayer,
			FirstPlayerIdx = MatchConfig.Default1x1.FirstPlayerIdx,
			ExhaustDamage = MatchConfig.Default1x1.ExhaustDamage,
			TeamSize = MatchConfig.Default1x1.TeamSize,
			TeamCount = MatchConfig.Default1x1.TeamCount,		
		} },
		{ "1 vs. 1", MatchConfig.Default1x1 },
		{ "2 vs. 2", MatchConfig.Default2x2 },
	};

	private static readonly List<IPlayerEditorResultCheck> PER_CHECKS = [
		new SameNamePlayerEditorResultCheck(),
		new SameDeckPlayerEditorResultCheck()
	];

	[Export]
	public LocalRepository RepoNode { get; set; }

    #region Packed scenes

	[ExportGroup("Packed scenes")]
    [Export]
    public PackedScene BotEditorScene { get; set; }
	[Export]
	public PackedScene LocalMatchScene { get; set; }

    #endregion

	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public OptionButton PresetOptionNode { get; set; }
	[Export]
	public MatchConfigEditor MatchConfigEditorNode { get; set; }
	[Export]
	public Label CantStartReasonNode { get; set; }
    [Export]
    public PlayerEditor RealPlayerEditor { get; set; }
    [Export]
    public Container BotListContainer { get; set; }
	[Export]
	public TabContainer TabsNode { get; set; }
	[Export]
	public AcceptDialog CantStartMatchDialogNode { get; set; }

	#endregion

	public override void _Ready()
	{
		CantStartReasonNode.Hide();
		PresetOptionNode.Clear();
		foreach (var pair in PRESETS)
		{
			PresetOptionNode.AddItem(pair.Key);
		}

		RemoveBotNodes();
		AddBot();

		OnPresetOptionItemSelected(PresetOptionNode.Selected);

		RealPlayerEditor.LoadLocalMatchesTab(this);
		RealPlayerEditor.LoadName("You");
	}

	public void UpdateDeckOptions()
	{
		List<IPlayerEditor> playerEditors = [
			RealPlayerEditor,
			.. BotListContainer.GetChildren().Cast<BotEditor>()
		];

		foreach (var per in playerEditors)
		{
			per.UpdateDeckLists();
		}
	}

    private void RemoveBotNodes()
    {
        while (BotListContainer.GetChildCount() > 0)
            BotListContainer.RemoveChild(BotListContainer.GetChild(0));
    }
	
	private Match CreateMatch()
	{
		// read core from DB
		var core = RepoNode.GetCore();
		if (core is null)
		{
			// TODO display AcceptDialog
			return null;
		}

		// create map
		// TODO

		var map = MapTemplate.GetBaskervilleTemplate();

		// create config
		var config = MatchConfigEditorNode.Build();

		// create players
		var match = new Match(config, map, core.Text)
		{
			Logger = new GDLogger()
			// Logger = null
		};

		return match;
	}

	private void AddBot()
	{
		var child = BotEditorScene.Instantiate<BotEditor>();
		BotListContainer.AddChild(child);

		child.LoadLocalMatchesTab(this);
		child.LoadName($"Bot{BotListContainer.GetChildCount()}");
	}

	#region Signal connections

	public void OnPresetOptionItemSelected(int idx)
	{
		var config = PRESETS[PresetOptionNode.GetItemText(idx)];
		MatchConfigEditorNode.Load(config);
	}

	public void OnStartMatchButtonPressed()
	{
		var match = CreateMatch();
		if (match is null) return;
		
		// var realController = new LocalMatchIOHandler(this);
		var child = LocalMatchScene.Instantiate<LocalMatch>();

		List<PlayerEditorResult> pers = [];
		var rpPer = RealPlayerEditor.Build();
		var handler = new LocalMatchIOHandler(child);
		rpPer = new PlayerEditorResult()
		{
			Controller = new IOPlayerController(handler),
			Loadout = rpPer.Loadout,
			Name = rpPer.Name,
			TeamIdx = rpPer.TeamIdx,
			Textures = rpPer.Textures,
		};
		pers.Add(rpPer);
		foreach (var playerEditor in BotListContainer.GetChildren().Cast<BotEditor>())
		{
			var build = playerEditor.Build();
			pers.Add(build);
		}

		// TODO check all players
		var startMatch = true;
		var errors = new List<string>();
		foreach (var check in PER_CHECKS)
		{
			var error = check.Check(pers);
			if (string.IsNullOrEmpty(error)) continue;

			errors.Add(error);
		}

		if (errors.Count > 0)
		{
			var errMsg = "Cant start match, errors:\n" + string.Join('\n', errors);
			CantStartMatchDialogNode.DialogText = errMsg;
			CantStartMatchDialogNode.Show();
			startMatch = false;
		}

		if (!startMatch)
		{
			child.QueueFree();
			return;
		}

		// start match
		child.Name = $"Match{TabsNode.GetChildCount()}";
		TabsNode.AddChild(child);
		TabsNode.MoveChild(child, 0);

		child.Show();
		child.Start(match, pers, handler);
	}

	public void OnAddBotButtonPressed()
	{
		if (BotListContainer.GetChildCount() == 3) return;

		AddBot();
	}

	#endregion
}
