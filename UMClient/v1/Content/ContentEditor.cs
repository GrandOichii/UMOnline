using Godot;
using System;
using System.Linq;

public partial class ContentEditor : Control
{
	#region Nodes

	[Export]
	public LocalRepository RepositoryNode { get; set; }
	[Export]
	public LocalMatchesTab LocalMatchesTabNode { get; set; }

	[ExportGroup("Nodes")]
	[Export]
	public DeckList OfficialDecks { get; set; }
	[Export]
	public DeckList CustomDecks { get; set; }
	[Export]
	public TabContainer DeckTabsNode { get; set; }
	[Export]
	public NameEditWindow NewDeckWindow { get; set; }
	[Export]
	public Container DeckListsContainer { get; set; }
	[Export]
	public Button CollapseDeckListsButton { get; set; }
	[Export]
	public ConfirmationDialog DeleteDeckDialogNode { get; set; }

	#endregion

	#region Packed scenes

	[ExportGroup("Packed scenes")]
	[Export]
	public PackedScene DeckEditorScene { get; set; }

	#endregion

	public void ReloadDeckLists()
	{
		OfficialDecks.LoadDecks();
		CustomDecks.LoadDecks();
	}

	public void UpdateDeckTabNames()
	{
		foreach (var node in DeckTabsNode.GetChildren().Cast<DeckEditor>())
		{
			var deckId = node.GetDeckId();
			var deck = RepositoryNode.GetDeck(deckId);
			node.Name = deck.Name;
		}
	}

	public override void _Ready()
	{
		RepositoryNode.ContentUpdateProcessed += OnRepositoryNodeContentUpdateProcessed;
		OfficialDecks.Repo = RepositoryNode;
		CustomDecks.Repo = RepositoryNode;

		// DecksFoldableContainer.horizo
		while (DeckTabsNode.GetChildCount() > 0)
			DeckTabsNode.RemoveChild(DeckTabsNode.GetChild(0));

		DeckTabsNode.GetTabBar().CloseWithMiddleMouse = true;
		DeckTabsNode.GetTabBar().TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowAlways;
		DeckTabsNode.GetTabBar().TabClosePressed += OnDeckTabsTabBarTabClosePressed;

		ReloadDeckLists();
	}

	private void OpenDeck(int deckId)
	{
		var deck = RepositoryNode.GetDeck(deckId);
		var existing = DeckTabsNode.GetNodeOrNull<Control>(deck.Name);
		if (existing is not null)
		{
			existing.Show();
			return;
		}

		var child = DeckEditorScene.Instantiate<DeckEditor>();
		DeckTabsNode.AddChild(child);
		child.Name = deck.Name;
		child.SetEssentials(
			this,
			RepositoryNode
		);

		child.LoadDeck(deck);

		child.Show();
	}

	public void UpdateLocalMatchesTab()
	{
		LocalMatchesTabNode.UpdateDeckOptions();
	}

	#region Signal connections

	public void OnDeckTabsTabBarTabClosePressed(long tabIdx)
	{
		DeckTabsNode.RemoveChild(DeckTabsNode.GetChild((int)tabIdx));
	}

	public void OnCreateDeckButtonPressed()
	{
		NewDeckWindow.SetEditData(
			RepositoryNode.GetDeckNames()
		);
		NewDeckWindow.Show();
	}

	private int _queuedForDeletionDeckId = -1;
	public void OnDeleteDeckButtonPressed()
	{
		var selected = CustomDecks.ListNode.GetSelectedItems();
        if (selected.Length != 1) return;

        var deckId = CustomDecks.ListNode.GetItemMetadata(selected[0]).AsInt32();
        _queuedForDeletionDeckId = deckId;
        var deck = RepositoryNode.GetDeck(deckId);
        DeleteDeckDialogNode.DialogText = $"Are you sure you want to delete deck {deck.Name}?";
        DeleteDeckDialogNode.Show();
	}

	
	public void OnDeleteDeckDialogConfirmed()
	{
		if (_queuedForDeletionDeckId == -1) throw new Exception($"{nameof(OnDeleteDeckDialogConfirmed)} was called with {nameof(_queuedForDeletionDeckId)} = -1");

        RepositoryNode.DeleteDeck(_queuedForDeletionDeckId);

        ReloadDeckLists();
        foreach (var tab in DeckTabsNode.GetChildren().Cast<DeckEditor>())
        {
            if (tab.DeckId != _queuedForDeletionDeckId) continue;
            tab.QueueFree();
        }

        _queuedForDeletionDeckId = -1;

		UpdateLocalMatchesTab();
	}

	public void OnOfficialDecksDeckActivated(int deckId)
	{
		OpenDeck(deckId);
	}

	public void OnCustomDecksDeckActivated(int deckId)
	{
		OpenDeck(deckId);
	}

	public void OnNewDeckWindowCancelRequest()
	{
		NewDeckWindow.Hide();
	}

	public void OnNewDeckWindowConfirmRequest(string deckName)
	{
		NewDeckWindow.Hide();

		var deck = new DeckModel()
		{
			Id = -1,
			Name = deckName,
			ChoosesSidekick = false,
			Editable = true,
			MaxHandSize = 7,
			StartingHandSize = 5,
			StartsWithSidekicks = true,
			Description = "",
			CardBackPath = null,
		};

		RepositoryNode.InsertDeck(deck);
		var inserted = RepositoryNode.GetDeck(deck.Name);
		OpenDeck(inserted.Id);
		ReloadDeckLists();

		UpdateLocalMatchesTab();
	}

	public void OnCollapseDeckListsButtonPressed()
	{
		var state = DeckListsContainer.Visible;
		CollapseDeckListsButton.Text = state ? ">" : "<";
		DeckListsContainer.Visible = !state; 
	}

	public void OnRepositoryNodeContentUpdateProcessed()
	{
		// remove all tabs
		while (DeckTabsNode.GetChildCount() > 0)
			DeckTabsNode.RemoveChild(DeckTabsNode.GetChild(0));
		
		// reload deck lists
		ReloadDeckLists();
	}

	#endregion
}
