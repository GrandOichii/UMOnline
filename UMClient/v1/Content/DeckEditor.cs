using Godot;
using System;

public partial class DeckEditor : Control
{
    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public DeckInfoEditor DeckInfoEditorNode { get; set; }

    #endregion

    private int _deckId;
    private LocalRepository _repo;

    public void SetEssentials(LocalRepository repo)
    {
        _repo = repo;

        DeckInfoEditorNode.SetEssentials(repo);
    }

    public void LoadDeck(DeckModel deck)
    {
        _deckId = deck.Id;

        DeckInfoEditorNode.LoadDeck(deck);
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

    #endregion
}
