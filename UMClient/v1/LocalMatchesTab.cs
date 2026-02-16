using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UMCore;
using UMCore.Matches;
using UMCore.Templates;

public partial class LocalMatchesTab : Control
{
	private static readonly Dictionary<string, MatchConfig> PRESETS = new()
	{
		{ "1 vs. 1", MatchConfig.Default1x1 },
		{ "2 vs. 2", MatchConfig.Default2x2 },
	};

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
		RealPlayerEditor.LoadName("RealPlayer"); // TODO? remove

		// TODO remove
		RealPlayerEditor.DeckOption.Select(6);
		RealPlayerEditor.TeamNode.Value = 2;
	}

    private void RemoveBotNodes()
    {
        while (BotListContainer.GetChildCount() > 0)
            BotListContainer.RemoveChild(BotListContainer.GetChild(0));
    }
	
	public static IEnumerable<MapNodeLinkTemplate> Bidirectional(MapNodeTemplate n1, MapNodeTemplate n2)
	{
		return [
			new() {
				First = n1.Id,
				Second = n2.Id,
			},
		];
	}
	
	public static MapTemplate GetBaskervilleTemplate()
	{
		List<MapNodeTemplate> nodes = [
			new() {
				Id = 0,
				Zones = [0],
				HasSecretPassage = true,
			},
			new() {
				Id = 1,
				Zones = [0],
			},
			new() {
				Id = 2,
				Zones = [0],
			},
			new() {
				Id = 3,
				Zones = [0],
			},
			new() {
				Id = 4,
				Zones = [0, 1],
				SpawnNumber = 2,
			},
			new() {
				Id = 5,
				Zones = [0, 6],
			},
			new() {
				Id = 6,
				Zones = [1],
			},
			new() {
				Id = 7,
				Zones = [1],
			},
			new() {
				Id = 8,
				Zones = [1],
			},
			new() {
				Id = 9,
				Zones = [1, 2, 3],
			},
			new() {
				Id = 10,
				Zones = [2],
			},
			new() {
				Id = 11,
				Zones = [2],
			},
			new() {
				Id = 12,
				Zones = [2],
				HasSecretPassage = true,
			},
			new() {
				Id = 13,
				Zones = [3, 4],
			},
			new() {
				Id = 14,
				Zones = [4],
			},
			new() {
				Id = 15,
				Zones = [4],
			},
			new() {
				Id = 16,
				Zones = [4],
				SpawnNumber = 1,
			},
			new() {
				Id = 17,
				Zones = [3],
				HasSecretPassage = true,
			},
			new() {
				Id = 18,
				Zones = [3],
			},
			new() {
				Id = 19,
				Zones = [3, 5],
				SpawnNumber = 3,
			},
			new() {
				Id = 20,
				Zones = [5],
			},
			new() {
				Id = 21,
				Zones = [5],
			},
			new() {
				Id = 22,
				Zones = [4, 5],
			},
			new() {
				Id = 23,
				Zones = [5],
			},
			new() {
				Id = 24,
				Zones = [5],
			},
			new() {
				Id = 25,
				Zones = [5],
			},
			new() {
				Id = 26,
				Zones = [5],
			},
			new() {
				Id = 27,
				Zones = [5],
				HasSecretPassage = true,
			},
			new() {
				Id = 28,
				Zones = [5, 6],
			},
			new() {
				Id = 29,
				Zones = [3, 6],
			},
			new() {
				Id = 30,
				Zones = [6],
			},
			new() {
				Id = 31,
				Zones = [6],
				SpawnNumber = 4,
			},
		];
		return new()
		{
			Nodes = nodes,
			Adjacent = [
				.. Bidirectional(nodes[0], nodes[2]),
				.. Bidirectional(nodes[0], nodes[1]),
				.. Bidirectional(nodes[4], nodes[1]),
				.. Bidirectional(nodes[4], nodes[5]),
				.. Bidirectional(nodes[4], nodes[6]),
				.. Bidirectional(nodes[7], nodes[6]),
				.. Bidirectional(nodes[7], nodes[8]),
				.. Bidirectional(nodes[7], nodes[9]),
				.. Bidirectional(nodes[8], nodes[9]),
				.. Bidirectional(nodes[10], nodes[9]),
				.. Bidirectional(nodes[11], nodes[9]),
				.. Bidirectional(nodes[13], nodes[9]),
				.. Bidirectional(nodes[13], nodes[14]),
				.. Bidirectional(nodes[15], nodes[14]),
				.. Bidirectional(nodes[15], nodes[16]),
				.. Bidirectional(nodes[13], nodes[16]),
				.. Bidirectional(nodes[13], nodes[17]),
				.. Bidirectional(nodes[13], nodes[21]),
				.. Bidirectional(nodes[22], nodes[21]),
				.. Bidirectional(nodes[22], nodes[23]),
				.. Bidirectional(nodes[22], nodes[15]),
				.. Bidirectional(nodes[21], nodes[23]),
				.. Bidirectional(nodes[18], nodes[17]),
				.. Bidirectional(nodes[11], nodes[10]),
				.. Bidirectional(nodes[12], nodes[10]),
				.. Bidirectional(nodes[13], nodes[20]),
				.. Bidirectional(nodes[21], nodes[20]),
				.. Bidirectional(nodes[19], nodes[20]),
				.. Bidirectional(nodes[19], nodes[24]),
				.. Bidirectional(nodes[19], nodes[18]),
				.. Bidirectional(nodes[29], nodes[18]),
				.. Bidirectional(nodes[29], nodes[28]),
				.. Bidirectional(nodes[27], nodes[28]),
				.. Bidirectional(nodes[27], nodes[26]),
				.. Bidirectional(nodes[25], nodes[26]),
				.. Bidirectional(nodes[25], nodes[19]),
				.. Bidirectional(nodes[25], nodes[24]),
				.. Bidirectional(nodes[28], nodes[30]),
				.. Bidirectional(nodes[31], nodes[30]),
				.. Bidirectional(nodes[2], nodes[30]),
				.. Bidirectional(nodes[31], nodes[5]),
				.. Bidirectional(nodes[24], nodes[20]),
				.. Bidirectional(nodes[6], nodes[8]),
				.. Bidirectional(nodes[3], nodes[5]),
				.. Bidirectional(nodes[3], nodes[2]),
			]
		};
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

		var map = GetBaskervilleTemplate();

		// create config
		var config = MatchConfigEditorNode.Build();

		// create players
		var match = new Match(config, map, core.Text)
		{
			Logger = new GDLogger()
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
