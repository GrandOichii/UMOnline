using Godot;
using System;
using UMCore.Matches.Players;
using UMCore.Templates;

public partial class PlayerEditor : HBoxContainer, IPlayerEditor
{
    #region Nodes

    [Export]
    public LineEdit NameEditNode { get; set; }
    [Export]
    public OptionButton DeckOption { get; set; }
    [Export]
    public SpinBox TeamNode { get; set; }

    #endregion

    public LocalMatchesTab LMT { get; private set; }

    public PlayerEditorResult Build()
    {
        return new()
        {
            Name = NameEditNode.Text,
            TeamIdx = (int)TeamNode.Value - 1,
            Controller = null,
            Loadout = GetLoadoutTemplate(),
            Textures = GetTextures()
        };
    }

    private ImageMaps? GetTextures()
    {
        var idx = DeckOption.Selected;
        var deckId = DeckOption.GetItemMetadata(idx).As<int>();

        var deck = LMT.RepoNode.GetDeck(deckId);
        if (!deck.Editable) return null;

        return new()
        {
            CardBack = LMT.RepoNode.GetCardBackTexture(deckId),  
            Cards = LMT.RepoNode.GetCardTextureMap(deckId),  
            Fighters = LMT.RepoNode.GetFighterTextureMap(deckId),  
        };
    }

    private LoadoutTemplate GetLoadoutTemplate()
    {
        var idx = DeckOption.Selected;
        var deckId = DeckOption.GetItemMetadata(idx).As<int>();

        return LMT.RepoNode.GetLoadoutTemplate(deckId);
    }

    public void LoadLocalMatchesTab(LocalMatchesTab lmt)
    {
        LMT = lmt;

        var officialDecks = lmt.RepoNode.GetDecks(false);
        foreach (var deck in officialDecks)
        {
            DeckOption.AddItem(deck.Name);
            DeckOption.SetItemMetadata(DeckOption.ItemCount - 1, deck.Id);
        }
        DeckOption.AddSeparator();

        var customDecks = lmt.RepoNode.GetDecks(true);
        foreach (var deck in customDecks)
        {
            DeckOption.AddItem(deck.Name);
            DeckOption.SetItemMetadata(DeckOption.ItemCount - 1, deck.Id);
        }
    }

    public void LoadName(string name)
    {
        NameEditNode.Text = name;
    }

}
