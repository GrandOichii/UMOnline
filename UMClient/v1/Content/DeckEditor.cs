using Godot;
using System;
using System.Diagnostics;
using System.Linq;

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
    [Export]
    public NameEditWindow NewFighterWindow { get; set; }
    // [Export]
    // public ItemList CardListNode { get; set; }
    // [Export]
    // public Button CreateCardButton { get; set; }
    // [Export]
    // public Button DeleteCardButton { get; set; }
    // [Export]
    // public NameEditWindow NewCardWindow { get; set; }

    #endregion

    #region Packed scenes

    [ExportGroup("Packed scenes")]
    [Export]
    public PackedScene FighterEditorScene { get; set; }

    #endregion

    private int _deckId;
    private LocalRepository _repo;
    private ContentEditor _parent;

    public override void _Ready()
    {
        base._Ready();

        FighterTabsNode.GetTabBar().CloseWithMiddleMouse = true;
        FighterTabsNode.GetTabBar().TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowAlways;
        FighterTabsNode.GetTabBar().TabClosePressed += OnFighterTabsTabBarTabClosePressed;

        // CardTabsNode.GetTabBar().CloseWithMiddleMouse = true;
        // CardTabsNode.GetTabBar().TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowAlways;
        // CardTabsNode.GetTabBar().TabClosePressed += OnCardTabsTabBarTabClosePressed;

        while (FighterTabsNode.GetChildCount() > 0)
            FighterTabsNode.RemoveChild(FighterTabsNode.GetChild(0));
        // while (CardsTabsNode.GetChildCount() > 0)
        //     CardsTabsNode.RemoveChild(CardsTabsNode.GetChild(0));
    }


    public int GetDeckId() => _deckId;

    public void SetEssentials(ContentEditor parent, LocalRepository repo)
    {
        _parent = parent;
        _repo = repo;

        DeckInfoEditorNode.SetEssentials(repo);
    }

    public void SetIsEditable(bool v)
    {
        CreateFighterButton.Disabled = !v;
        DeleteFighterButton.Disabled = !v;
        // CreateCardButton.Disabled = !v;
        // DeleteCardButton.Disabled = !v;
    }

    public void LoadDeck(DeckModel deck)
    {
        _deckId = deck.Id;
        SetIsEditable(deck.Editable);

        DeckInfoEditorNode.LoadDeck(deck);
        ReloadFighterList();
        LoadCardList();
    }

    public void ReloadFighterList()
    {
        FighterListNode.Clear();

        var fighters = _repo.GetFighters(_deckId);
        foreach (var fighter in fighters)
        {
            var idx = FighterListNode.AddItem(fighter.Name);
            FighterListNode.SetItemMetadata(idx, fighter.Id);
        }
    }

    public void LoadCardList()
    {
        // CardListNode.Clear();

        // TODO
    }

    public void OpenFighter(int fighterId)
    {
        var fighter = _repo.GetFighter(fighterId);
        var existing = FighterTabsNode.GetNodeOrNull<Control>(fighter.Name);
        if (existing is not null)
        {
            existing.Show();
            return;
        }

        var child = FighterEditorScene.Instantiate<FighterEditor>();
        FighterTabsNode.AddChild(child);
        child.Name = fighter.Name;
        // TODO
        // child.SetEssentials(
        //     this,
        //     _repo
        // );

        var deck = _repo.GetDeck(fighter.DeckId);
        child.LoadFighter(fighter, deck);

        child.FighterChanged += OnFighterEditorFighterChanged;
        child.FighterNameChanged += OnFighterEditorFighterNameChanged;
        child.FighterImageImportRequest += OnFighterImageImportRequest;

        child.Show();
    }

    private FighterEditor GetOpenedFighterEditor(int fighterId)
    {
        foreach (var editor in FighterTabsNode.GetChildren().Cast<FighterEditor>())
        {
            if (editor.GetFighterId() != fighterId) continue;
            return editor;
        }

        throw new Exception($"Failed to find opnened fighter editor for fighterId = {fighterId}");
    }

    private void UpdateDeckTabNames()
    {
        foreach (var node in FighterTabsNode.GetChildren().Cast<FighterEditor>())
        {
            var fighterId = node.GetFighterId();
            var fighter = _repo.GetFighter(fighterId);
            node.Name = fighter.Name;
        }
    }

    #region Signal connections

    public void OnFighterEditorFighterNameChanged(int _fighterId)
    {
        ReloadFighterList();
        UpdateDeckTabNames();
    }

    public void OnFighterEditorFighterChanged(int fighterId)
    {
        var editor = GetOpenedFighterEditor(fighterId);
        var fighter = editor.BuildFighterModel();
        _repo.UpdateFighterById(fighter);
    }

    public void OnFighterImageImportRequest(int fighterId, string path)
    {
        var editor = GetOpenedFighterEditor(fighterId);
        var newPath = _repo.UpdateFighterImage(editor.BuildFighterModel().Id, path);

        editor.UpdateFighterImage(newPath);
        // TODO
    }

    public void OnDeckDeckInfoChanged()
    {
        var deck = DeckInfoEditorNode.BuildDeckModel();
        Debug.Assert(deck.Id == _deckId);
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

    public void OnCreateFighterButtonPressed()
    {
        NewFighterWindow.SetEditData(
            _repo.GetFighterNames(_deckId)
        );
        NewFighterWindow.Show();
    }

    public void OnDeleteFighterButtonPressed()
    {
        // TODO
    }

    public void OnNewFighterWindowCancelRequest()
    {
        NewFighterWindow.Hide();
    }

    public void OnNewFighterWindowConfirmRequest(string fighterName)
    {
        NewFighterWindow.Hide();

        var fighter = new FighterModel()
        {
            Id = -1,
            Name = fighterName,
            Amount = 1,
            CanMoveOverOpposing = false,
            DeckId = _deckId,
            IsRanged = false,
            IsSidekick = false,
            IsSmall = false,
            MaxHealth = 10,
            StartingHealth = 10,
            MeleeRange = 1,
            Movement = 2,
            Text = "",
            ImagePath = null,
        };

        _repo.InsertModel(fighter);
        var inserted = _repo.GetFighter(fighter.Name, _deckId);
        OpenFighter(inserted.Id);
        ReloadFighterList();
    }

    public void OnFighterListItemActivated(int idx)
    {
        var fighterId = FighterListNode.GetItemMetadata(idx).AsInt32();

        OpenFighter(fighterId);
    }

    public void OnFighterTabsTabBarTabClosePressed(long tabIdx)
    {
        FighterTabsNode.RemoveChild(FighterTabsNode.GetChild((int)tabIdx));
    }

    // public void OnCardTabsTabBarTabClosePressed(long tabIdx)
    // {
    //     CardTabsNode.RemoveChild(CardTabsNode.GetChild((int)tabIdx));
    // }


    #endregion
}
