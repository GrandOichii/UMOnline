using Godot;
using System;

public partial class DeckList : VBoxContainer
{
	#region Signals

	[Signal]
	public delegate void DeckActivatedEventHandler(int deckId);

	#endregion


	[Export]
	public string Title { get; set; }
	[Export]
	public bool PickEditableDecks { get; set; }
	[Export]
	public LocalRepository Repo { get; set; }

	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public ItemList ListNode { get; set; }
	[Export]
	public Label TitleNode { get; set; }

	#endregion

	public override void _Ready()
	{
		TitleNode.Text = Title;
	}

	public void LoadDecks()
	{
		ListNode.Clear();

		var decks = Repo.GetDecks(PickEditableDecks);        
		foreach (var deck in decks)
		{
			var idx = ListNode.AddItem(deck.Name);
			ListNode.SetItemMetadata(idx, deck.Id);
		}
	}

	#region Signal connections

	public void OnItemListItemActivated(int idx)
	{
		var deckId = ListNode.GetItemMetadata(idx).AsInt32();
		EmitSignalDeckActivated(deckId);
	}

	#endregion
}
