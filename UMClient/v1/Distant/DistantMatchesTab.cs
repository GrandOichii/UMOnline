using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using UMCore.Templates;
using UMDTO;
using UMModel.Models;

public partial class DistantMatchesTab : Control
{
	[Export]
	public string DefaultAddress { get; set; }
	[Export]
	public Texture2D ConnectButtonTexture { get; set; }
	[Export]
	public Texture2D ReplayButtonTexture { get; set; }

	#region Nodes

	[Export]
	public LocalRepository RepoNode { get; set; }
	[Export]
	public ServerConnection ServerConnectionNode { get; set; }
	[ExportGroup("Nodes")]
	[Export]
	public Control ConnectionFormNode { get; set; }
	[Export]
	public Control ConnectionDisplayNode { get; set; }
	[Export]
	public LineEdit ServerAddressEditNode { get; set; }
	[Export]
	public LineEdit NameEditNode { get; set; }
	[Export]
	public AcceptDialog ConnectionErrorDialogNode { get; set; }
	[Export]
	public Button ConnectButtonNode { get; set; }
	[Export]
	public AcceptDialog OutdatedContentDialogNode { get; set; }
	[Export]
	public Window ContentSyncWaitDialogNode { get; set; }
	[Export]
	public AcceptDialog ContentUpdateFailDialogNode { get; set; }
	[Export]
	public AcceptDialog FinishedContentUpdateDialog { get; set; }
	[Export]
	public Tree ActiveMatchesTableNode { get; set; }
	[Export]
	public Container AllowedDecksContainerNode { get; set; }
	[Export]
	public OptionButton NewMatchConfigOptionNode { get; set; }
	[Export]
	public Button CreateMatchButtonNode { get; set; }
	[Export]
	public LineEdit NewMatchTitleEditNode { get; set; }
	[Export]
	public Node DistantMatchWindowsNode { get; set; }
	[Export]
	public Tree FinishedMatchesTableNode { get; set; }
	[Export]
	public Node ReplayWindowsNode { get; set; }
	[Export]
	public AcceptDialog MatchErrorDialogNode { get; set; }
	[Export]
	public AcceptDialog ContentUpdateFailureDialogNode { get; set; }

	#endregion

	#region Packed scenes

	[ExportGroup("Packed scenes")]
	[Export]
	public PackedScene DistantMatchWindowScene { get; set; }
	[Export]
	public PackedScene MatchReplayWindowScene { get; set; }

	#endregion

	private List<MatchConfig> _loadedConfigs = null;
	private List<MatchProcessGet> _activeMatches = null;
	private List<MatchProcessGet> _finishedMatches = null;

	private void CheckCanPressConnect()
	{
		ConnectButtonNode.Disabled = true;
		if (ServerAddressEditNode.Text.Length == 0) return;
		if (NameEditNode.Text.Length == 0) return;

		ConnectButtonNode.Disabled = false;
	}

	public override void _Ready()
	{
		#region ActiveMatchesTableNode configuration

		ActiveMatchesTableNode.Columns = 4;
		ActiveMatchesTableNode.SetColumnTitle(0, "Id");
		ActiveMatchesTableNode.SetColumnTitle(1, "Title");
		ActiveMatchesTableNode.SetColumnTitle(2, "Status");
		ActiveMatchesTableNode.SetColumnTitle(3, "Connect");
		ActiveMatchesTableNode.SetColumnExpandRatio(3, 1);

		#endregion

		#region FinishedMatchesTableNode configuration

		FinishedMatchesTableNode.Columns = 4;
		FinishedMatchesTableNode.SetColumnTitle(0, "Id");
		FinishedMatchesTableNode.SetColumnTitle(1, "Title");
		FinishedMatchesTableNode.SetColumnTitle(2, "Status");
		FinishedMatchesTableNode.SetColumnTitle(3, "Replay");

		// FinishedMatchesTableNode.CreateItem(); // root

		// var item = FinishedMatchesTableNode.CreateItem();
		// FillFinishedMatchTreeItem(item, new()
		// {
		//     Id = Guid.NewGuid().ToString(),
		//     AllowedFighters = [],
		//     Players = [],
		//     Status = MatchProcessGetStatus.FINISHED,
		//     TeamCount = 2,
		//     Title = "match1"
		// }, 0);

		#endregion

		ServerConnectionNode.ContentUpdateFinished += OnServerConnectionNodeContentUpdateFinished;
		ServerConnectionNode.ContentUpdateFailed += OnServerConnectionNodeContentUpdateFailed;
		ServerConnectionNode.ContentOutdatedResponded += OnServerConnectionNodeContentOutdatedResponded;

		ConnectionDisplayNode.Hide();
		ConnectionFormNode.Show();

		var state = RepoNode.GetAppState();

		ServerAddressEditNode.Text = state.LastConnectedAddress ?? DefaultAddress;
		NameEditNode.Text = state.LastUsedName;

		CreateMatchButtonNode.Disabled = true;

		CheckCanPressConnect();
	}

	private void FillActiveMatchTreeItem(TreeItem item, MatchProcessGet match, int id, bool canConnect)
	{
		item.SetText(0, match.Id);
		item.SetText(1, match.Title);
		item.SetText(2, match.Status switch
		{
			MatchProcessGetStatus.WAITING_FOR_PLAYERS => "Waiting for players",
			MatchProcessGetStatus.IN_PROGRESS => "In progress",
			_ => throw new Exception($"Cannot display match with status: {match.Status}")
		});
		item.AddButton(3, ConnectButtonTexture, id, !canConnect);
		item.SetTextAlignment(3, HorizontalAlignment.Center);
	}

	private void FillFinishedMatchTreeItem(TreeItem item, MatchProcessGet match, int id)
	{
		item.SetText(0, match.Id);
		item.SetText(1, match.Title);
		item.SetText(2, match.Status switch
		{
			MatchProcessGetStatus.FINISHED => "Finished",
			MatchProcessGetStatus.CRASHED => "Crashed",
			_ => throw new Exception($"Cannot display match with status: {match.Status}")
		});
		item.AddButton(3, ReplayButtonTexture, id);
	}

	private void CheckContent()
	{
		var state = RepoNode.GetAppState();
		if (state.LastUpdateDT is null)
		{
			OutdatedContentDialogNode.Show();
			return;
		}

		ServerConnectionNode.RequestIsOutdated((DateTime)state.LastUpdateDT);
	}

	private void UpdateCurrentMatches(List<MatchProcessGet> matches)
	{
		var matchMap = matches.ToDictionary(m => m.Id);
		foreach (var child in DistantMatchWindowsNode.GetChildren().Cast<DistantMatchWindow>())
		{
			if (!matchMap.TryGetValue(child.MatchId, out var mpg))
			{
				// TODO
				GD.Print($"Match {child.MatchId} is no longer tracked by server!");
				return;
			}

			child.Update(mpg);
		}
	}

	private void UpdateActiveTables(List<MatchProcessGet> matches)
	{
		ActiveMatchesTableNode.Clear();
		ActiveMatchesTableNode.CreateItem(); // root
		// for (int i = 0; i < 3; ++i)
		// {
		var myMatches = DistantMatchWindowsNode
			.GetChildren()
			.Cast<DistantMatchWindow>()
			.Select(w => w.MatchId)
			.ToList();

		_activeMatches = [];
		foreach (var match in matches)
		{
			if (
				match.Status != MatchProcessGetStatus.WAITING_FOR_PLAYERS &&
				match.Status != MatchProcessGetStatus.IN_PROGRESS
			) continue;

			var item = ActiveMatchesTableNode.CreateItem();

			var canConnect = true;
			if (match.Status == MatchProcessGetStatus.IN_PROGRESS)
			{
				canConnect = false;
			}
			if (myMatches.Contains(match.Id))
			{
				canConnect = false;
			}

			FillActiveMatchTreeItem(item, match, _activeMatches.Count, canConnect);
			_activeMatches.Add(match);;
		}

		// }
	}

	private void UpdateFinishedTables(List<MatchProcessGet> matches)
	{
		return;
		FinishedMatchesTableNode.Clear();
		FinishedMatchesTableNode.CreateItem(); // root

		_finishedMatches = [];
		foreach (var match in matches)
		{
			if (
				match.Status != MatchProcessGetStatus.FINISHED &&
				match.Status != MatchProcessGetStatus.CRASHED
			) continue;

			var item = FinishedMatchesTableNode.CreateItem();
			FillFinishedMatchTreeItem(item, match, _finishedMatches.Count);
		}
	}

	private void OnUpdateTables(List<MatchProcessGet> matches)
	{
		Callable.From(() =>
		{
			UpdateActiveTables(matches);
			UpdateFinishedTables(matches);
			UpdateCurrentMatches(matches);
		}).CallDeferred();
	}

	private List<string> GetAllowedDecks()
	{
		return [.. AllowedDecksContainerNode
			.GetChildren()
			.Cast<CheckBox>()
			.Where(c => c.ButtonPressed)
			.Select(c => c.Text)];
	}

	private MatchConfig GetPickedConfig()
	{
		var idx = NewMatchConfigOptionNode.Selected;
		var text = NewMatchConfigOptionNode.GetItemText(idx);
		var config = _loadedConfigs.Single(c => c.Name == text);
		return config;
	}

	private void CheckCanCreateMatch()
	{
		if (NewMatchTitleEditNode.Text.Length == 0)
		{
			CreateMatchButtonNode.Disabled = true;    
			return;
		}
		
		var allowed = GetAllowedDecks();
		var config = GetPickedConfig();

		CreateMatchButtonNode.Disabled = config.TeamCount * config.TeamSize > allowed.Count;
	}

	private async Task ConnectToMatch(string matchId, bool clientIsOwner)
	{
		
		var connectEndpoint = await ServerConnectionNode.ConnectToMatch(matchId);
		if (connectEndpoint.StartsWith("err:"))
		{
			MatchErrorDialogNode.Title = "Failed to connect";
			MatchErrorDialogNode.DialogText = $"Failed to connect to match with Id = {matchId}\nError: {connectEndpoint}";
			MatchErrorDialogNode.Show();
			CreateMatchButtonNode.Disabled = false;
			return;
		}

		var socket = await ServerConnectionNode.WSConnect(connectEndpoint);

		var window = DistantMatchWindowScene.Instantiate<DistantMatchWindow>();
		window.SetEssentials(
			clientIsOwner,
			ServerConnectionNode,
			socket,
			matchId
		);
		DistantMatchWindowsNode.AddChild(window);

		await ServerConnectionNode.ForceTableUpdate();
	}

	#region Signal connections

	public async void OnConnectButtonPressed()
	{
		SetConnectionFormEditable(false);

		var registrationError = await ServerConnectionNode.Connect(
			ServerAddressEditNode.Text,
			NameEditNode.Text,
			OnUpdateTables
		);
		if (!string.IsNullOrEmpty(registrationError))
		{
			SetConnectionFormEditable(true);
			ConnectionErrorDialogNode.DialogText = $"Failed to connect!\n{registrationError}";
			ConnectionErrorDialogNode.Show();
			return;
		}

		// save name
		var appState = RepoNode.GetAppState();
		appState.LastUsedName = NameEditNode.Text;
		appState.LastConnectedAddress = ServerAddressEditNode.Text;
		RepoNode.UpdateAppState(appState);

		// show connection display
		ConnectionFormNode.Hide();
		ConnectionDisplayNode.Show();

		// load content for match creation
		_loadedConfigs = await ServerConnectionNode.FetchConfigs();
		LoadConfigs();
		var loadouts = await ServerConnectionNode.FetchLoadouts();
		LoadLoadouts(loadouts);
		CheckCanCreateMatch();

		CheckContent();
	}

	private void LoadConfigs()
	{
		NewMatchConfigOptionNode.Clear();
		foreach (var config in _loadedConfigs)
		{
			NewMatchConfigOptionNode.AddItem(config.Name);
		}
	}

	private void LoadLoadouts(List<LoadoutTemplate> loadouts)
	{
		while (AllowedDecksContainerNode.GetChildCount() > 0)
			AllowedDecksContainerNode.RemoveChild(AllowedDecksContainerNode.GetChild(0));

		foreach (var l in loadouts)
		{
			var child = new CheckBox()
			{
				Text = l.Name,
				ButtonPressed = true
			};
			child.Toggled += OnAllowedDeckCheckBoxToggled;
			AllowedDecksContainerNode.AddChild(child);
		}
	}

	private void SetConnectionFormEditable(bool v)
	{
		ServerAddressEditNode.Editable = v;
		NameEditNode.Editable = v;
		ConnectButtonNode.Disabled = !v;
	}

	public void OnServerAddressEditTextChanged(string _)
	{
		CheckCanPressConnect();
	}

	public void OnNameEditTextChanged(string _)
	{
		CheckCanPressConnect();
	}

	public void OnSyncContentButtonPressed()
	{
		ContentSyncWaitDialogNode.Show();
		ServerConnectionNode.RequestContentSynchronization();
	}

	public void OnServerConnectionNodeContentUpdateFinished()
	{
		var content = ServerConnectionNode.PopContentUpdate();
		RepoNode.ProcessContentUpdate(content);

		var state = RepoNode.GetAppState();
		state.LastUpdateDT = DateTime.Now.ToUniversalTime();
		RepoNode.UpdateAppState(state);

		ContentSyncWaitDialogNode.Hide();
		FinishedContentUpdateDialog.Show();
	}

	public void OnServerConnectionNodeContentUpdateFailed(string errMsg)
	{
		ContentUpdateFailureDialogNode.DialogText = "Failed to update content!\nError: {errMsg}";
		ContentUpdateFailureDialogNode.Show();
	}

	public void OnServerConnectionNodeContentOutdatedResponded(bool isOutdated)
	{
		if (!isOutdated) return;

		OutdatedContentDialogNode.Show();
	}

	public void OnAllowedFightersFilterEditTextChanged(string newText)
	{
		foreach (var child in AllowedDecksContainerNode.GetChildren().Cast<CheckBox>())
		{
			child.Visible = child.Text.Contains(newText, StringComparison.CurrentCultureIgnoreCase);
		}
	}

	public void OnSelectAllButtonPressed()
	{
		foreach (var child in AllowedDecksContainerNode.GetChildren().Cast<CheckBox>())
		{
			child.ButtonPressed = true;
		}
	}

	public void OnDeselectAllButtonPressed()
	{
		foreach (var child in AllowedDecksContainerNode.GetChildren().Cast<CheckBox>())
		{
			child.ButtonPressed = false;
		}
	}

	public void OnAllowedDeckCheckBoxToggled(bool _)
	{
		CheckCanCreateMatch();
	}

	public void OnNewMatchTitleEditTextChanged(string _)
	{
		CheckCanCreateMatch();
	}

	public async void OnCreateMatchButtonPressed()
	{
		CreateMatchButtonNode.Disabled = true;

		var createParams = new CreateMatchParams()
		{
			MatchConfigName = GetPickedConfig().Name,
			Title = NewMatchTitleEditNode.Text,
			AllowedLoadouts = GetAllowedDecks()
		};

		var matchId = await ServerConnectionNode.CreateMatch(createParams);
		if (matchId.StartsWith("err:"))
		{
			MatchErrorDialogNode.Title = "Failed to create";
			MatchErrorDialogNode.DialogText = $"Failed to create match!\nError: {matchId}";
			MatchErrorDialogNode.Show();
			CreateMatchButtonNode.Disabled = false;
			return;
		}

		await ConnectToMatch(matchId, true);
	}

	public async void OnActiveMatchesTableButtonClicked(TreeItem item, int column, int id, MouseButton mouseButton)
	{
		if (mouseButton != MouseButton.Left) return;
		if (column != 3) return;

		var matchId = _activeMatches[id].Id;

		await ConnectToMatch(matchId, false);
	}

	public async void OnFinishedMatchesTableButtonClicked(TreeItem item, int column, int id, MouseButton mouseButton)
	{
		if (mouseButton != MouseButton.Left) return;
		if (column != 3) return;
		
		var matchId = _finishedMatches[id].Id;
		if (ServerConnectionNode.IsOutdated)
		{
			OutdatedContentDialogNode.Show();
			return;
		}

		MatchRecordGet record;
		try
		{
			record = await ServerConnectionNode.GetRecord(matchId);
			if (record is null) return;
		} catch (Exception e)
		{
			return;
		}

		var window = MatchReplayWindowScene.Instantiate<MatchReplayWindow>();
		ReplayWindowsNode.AddChild(window);

		window.LoadMatchRecord(
			RepoNode,
			record
		);
	}

	public async void OnTestRecordPressed()
	{
		var record = await ServerConnectionNode.GetRecord("1");


		var window = MatchReplayWindowScene.Instantiate<MatchReplayWindow>();
		ReplayWindowsNode.AddChild(window);

		window.LoadMatchRecord(
			RepoNode,
			record
		);
	}

	#endregion
}
