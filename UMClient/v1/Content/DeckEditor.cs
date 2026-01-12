using Godot;
using System;

public partial class DeckEditor : Control
{
    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public DeckInfoEditor DeckInfoEditorNode { get; set; }
    [Export]
    public TabContainer FighterTabsNode { get; set; }
    [Export]
    public ItemList FighterListNode { get; set; }
    [Export]
    public Button CreateFighterButton { get; set; }
    [Export]
    public Button DeleteFighterButton { get; set; }

    #endregion

    private int _deckId;
    private LocalRepository _repo;
    private ContentEditor _parent;

    public int GetDeckId() => _deckId;

    public void SetEssentials(ContentEditor parent, LocalRepository repo)
    {
        _parent = parent;
        _repo = repo;

        DeckInfoEditorNode.SetEssentials(repo);
    }

    public void LoadDeck(DeckModel deck)
    {
        _deckId = deck.Id;

        DeckInfoEditorNode.LoadDeck(deck);
        LoadFighters();
        LoadCards();
    }

    public void LoadFighters()
    {
        // TODO
    }

    public void LoadCards()
    {
        // TODO
    }

    #region Signal connections

    public void OnDeckDeckInfoChanged()
    {
        var deck = DeckInfoEditorNode.GetDeck();
        deck.Id = _deckId;
        _repo.UpdateDeckById(deck);
    }

    public void OnDeckCardBackImportRequest(string path)
    {
        var newPath = _repo.UpdateDeckCardBack(_deckId, path);
        DeckInfoEditorNode.UpdateCardBack(newPath);
    }

    public void OnDeckDeckNameChanged()
    {
        _parent.ReloadDeckLists();
        _parent.UpdateDeckTabNames();
    }

    #endregion
}
