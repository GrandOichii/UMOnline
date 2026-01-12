using Godot;
using System;

public partial class ContentEditor : Control
{
    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public DeckList OfficialDecks { get; set; }
    [Export]
    public DeckList CustomDecks { get; set; }
    [Export]
    public LocalRepository RepositoryNode { get; set; }
    [Export]
    public TabContainer DeckTabsNode { get; set; }
    [Export]
    public NameEditWindow NewDeckWindow { get; set; }

    #endregion

    #region Packed scenes

    [ExportGroup("Packed scenes")]
    [Export]
    public PackedScene DeckEditorScene { get; set; }

    #endregion

    public override void _Ready()
    {
        while (DeckTabsNode.GetChildCount() > 0)
            DeckTabsNode.RemoveChild(DeckTabsNode.GetChild(0));

        DeckTabsNode.GetTabBar().CloseWithMiddleMouse = true;
        DeckTabsNode.GetTabBar().TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowAlways;
        DeckTabsNode.GetTabBar().TabClosePressed += OnDeckTabsTabBarTabClosePressed;
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
            RepositoryNode
        );

        child.LoadDeck(deck);

        child.Show();
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

    public void OnDeleteDeckButtonPressed()
    {
        // TODO
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
            ChoosesSidekick = true,
            Editable = true,
            MaxHandSize = 7,
            StartingHandSize = 5,
            StartsWithSidekicks = true,
            Description = "",
            CardBackPath = null,
        };

        RepositoryNode.InsertModel(deck);
        var inserted = RepositoryNode.GetDeck(deck.Name);
        OpenDeck(inserted.Id);
    }

    #endregion
}
