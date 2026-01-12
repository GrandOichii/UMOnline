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
    }

    public void LoadDeck(DeckModel deck)
    {
        _deckId = deck.Id;

        DeckInfoEditorNode.LoadDeck(deck);
    }
}
