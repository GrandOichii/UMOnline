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
    [Export]
    public TabContainer CardTabsNode { get; set; }
    [Export]
    public ItemList CardListNode { get; set; }
    [Export]
    public Button CreateCardButton { get; set; }
    [Export]
    public Button DeleteCardButton { get; set; }
    [Export]
    public NameEditWindow NewCardWindow { get; set; }
    [Export]
    public Container FighterListContainer { get; set; }
    [Export]
    public Button CollapseFighterListButton { get; set; }
    [Export]
    public Container CardListContainer { get; set; }
    [Export]
    public Button CollapseCardListButton { get; set; }
    [Export]
    public ConfirmationDialog DeleteFighterDialogNode { get; set; }
    [Export]
    public ConfirmationDialog DeleteCardDialogNode { get; set; }

    #endregion

    #region Packed scenes

    [ExportGroup("Packed scenes")]
    [Export]
    public PackedScene FighterEditorScene { get; set; }
    [Export]
    public PackedScene CardEditorScene { get; set; }

    #endregion

    public int DeckId { get; private set; }
    private LocalRepository _repo;
    private ContentEditor _parent;

    public override void _Ready()
    {
        base._Ready();

        FighterTabsNode.GetTabBar().CloseWithMiddleMouse = true;
        FighterTabsNode.GetTabBar().TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowAlways;
        FighterTabsNode.GetTabBar().TabClosePressed += OnFighterTabsTabBarTabClosePressed;

        CardTabsNode.GetTabBar().CloseWithMiddleMouse = true;
        CardTabsNode.GetTabBar().TabCloseDisplayPolicy = TabBar.CloseButtonDisplayPolicy.ShowAlways;
        CardTabsNode.GetTabBar().TabClosePressed += OnCardTabsTabBarTabClosePressed;

        while (FighterTabsNode.GetChildCount() > 0)
            FighterTabsNode.RemoveChild(FighterTabsNode.GetChild(0));
        while (CardTabsNode.GetChildCount() > 0)
            CardTabsNode.RemoveChild(CardTabsNode.GetChild(0));
    }


    public int GetDeckId() => DeckId;

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
        CreateCardButton.Disabled = !v;
        DeleteCardButton.Disabled = !v;
    }

    public void LoadDeck(DeckModel deck)
    {
        DeckId = deck.Id;
        SetIsEditable(deck.Editable);

        DeckInfoEditorNode.LoadDeck(deck);
        ReloadFighterList();
        ReloadCardList();
    }

    public void ReloadFighterList()
    {
        FighterListNode.Clear();

        var fighters = _repo.GetFighters(DeckId);
        foreach (var fighter in fighters)
        {
            var idx = FighterListNode.AddItem(fighter.Name);
            FighterListNode.SetItemMetadata(idx, fighter.Id);
        }
    }

    public void ReloadCardList()
    {
        CardListNode.Clear();

        var cards = _repo.GetCards(DeckId);
        foreach (var card in cards)
        {
            var idx = CardListNode.AddItem(card.Name);
            CardListNode.SetItemMetadata(idx, card.Id);
        }
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
        child.SetEssentials(
            () => _repo.GetFighterNames(DeckId)
        );

        var deck = _repo.GetDeck(fighter.DeckId);
        var script = _repo.GetScriptModel(fighter.ScriptId);
        child.LoadFighter(fighter, script, deck.Editable);

        child.FighterChanged += OnFighterEditorFighterChanged;
        child.FighterNameChanged += OnFighterEditorFighterNameChanged;
        child.FighterImageImportRequest += OnFighterImageImportRequest;
        child.FighterScriptChanged += OnFighterEditorFighterScriptChanged;

        child.Show();
    }

    public void OpenCard(int cardId)
    {
        var card = _repo.GetCard(cardId);
        var existing = CardTabsNode.GetNodeOrNull<Control>(card.Name);
        if (existing is not null)
        {
            existing.Show();
            return;
        }

        var child = CardEditorScene.Instantiate<CardEditor>();
        CardTabsNode.AddChild(child);
        child.Name = card.Name;
        child.SetEssentials(
            () => _repo.GetCardNames(DeckId)
        );

        var deck = _repo.GetDeck(card.DeckId);
        var script = _repo.GetScriptModel(card.ScriptId);
        child.LoadCard(card, script, deck.Editable);

        child.CardChanged += OnCardEditorCardChanged;
        child.CardNameChanged += OnCardEditorCardNameChanged;
        child.CardImageImportRequest += OnCardImageImportRequest;
        child.CardScriptChanged += OnCardEditorCardScriptChanged;

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

    private CardEditor GetOpenedCardEditor(int cardId)
    {
        foreach (var editor in CardTabsNode.GetChildren().Cast<CardEditor>())
        {
            if (editor.GetCardId() != cardId) continue;
            return editor;
        }

        throw new Exception($"Failed to find opnened card editor for cardId = {cardId}");
    }

    private void UpdateFighterTabNames()
    {
        foreach (var node in FighterTabsNode.GetChildren().Cast<FighterEditor>())
        {
            var fighterId = node.GetFighterId();
            var fighter = _repo.GetFighter(fighterId);
            node.Name = fighter.Name;
        }
    }

    private void UpdateCardTabNames()
    {
        foreach (var node in CardTabsNode.GetChildren().Cast<CardEditor>())
        {
            var cardId = node.GetCardId();
            var card = _repo.GetCard(cardId);
            node.Name = card.Name;
        }
    }

    #region Signal connections

    public void OnFighterEditorFighterNameChanged(int _fighterId)
    {
        ReloadFighterList();
        UpdateFighterTabNames();
    }

    public void OnFighterEditorFighterChanged(int fighterId)
    {
        var editor = GetOpenedFighterEditor(fighterId);
        var fighter = editor.BuildFighterModel();
        _repo.UpdateFighterById(fighter);
    }

    public void OnFighterEditorFighterScriptChanged(int fighterId)
    {
        var editor = GetOpenedFighterEditor(fighterId);
        var script = editor.BuildScriptModel();
        _repo.UpdateScriptById(script);
    }

    public void OnFighterImageImportRequest(int fighterId, string path)
    {
        var editor = GetOpenedFighterEditor(fighterId);
        var newPath = _repo.UpdateFighterImage(editor.BuildFighterModel().Id, path);

        editor.UpdateFighterImage(newPath);
    }

    public void OnCardEditorCardNameChanged(int _cardId)
    {
        ReloadCardList();
        UpdateCardTabNames();
    }

    public void OnCardEditorCardScriptChanged(int cardId)
    {
        var editor = GetOpenedCardEditor(cardId);
        var script = editor.BuildScriptModel();
        _repo.UpdateScriptById(script);
    }

    public void OnCardEditorCardChanged(int cardId)
    {
        var editor = GetOpenedCardEditor(cardId);
        var card = editor.BuildCardModel();
        _repo.UpdateCardById(card);
    }

    public void OnCardImageImportRequest(int cardId, string path)
    {
        var editor = GetOpenedCardEditor(cardId);
        var newPath = _repo.UpdateCardImage(editor.BuildCardModel().Id, path);

        editor.UpdateCardImage(newPath);
    }

    public void OnDeckDeckInfoChanged()
    {
        var deck = DeckInfoEditorNode.BuildDeckModel();
        Debug.Assert(deck.Id == DeckId);
        _repo.UpdateDeckById(deck);
    }

    public void OnDeckCardBackImportRequest(string path)
    {
        var newPath = _repo.UpdateDeckCardBack(DeckId, path);
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
            _repo.GetFighterNames(DeckId)
        );
        NewFighterWindow.Show();
    }

    private int _queuedForDeletionFighterId = -1;
    public void OnDeleteFighterButtonPressed()
    {
        var selected = FighterListNode.GetSelectedItems();
        if (selected.Length != 1) return;

        var fighterId = FighterListNode.GetItemMetadata(selected[0]).AsInt32();
        _queuedForDeletionFighterId = fighterId;
        var fighter = _repo.GetFighter(fighterId);
        DeleteFighterDialogNode.DialogText = $"Are you sure you want to delete fighter {fighter.Name}?";
        DeleteFighterDialogNode.Show();
    }

    public void OnDeleteFighterDialogConfirmed()
    {
        if (_queuedForDeletionFighterId == -1) throw new Exception($"{nameof(OnDeleteFighterDialogConfirmed)} was called with {nameof(_queuedForDeletionFighterId)} = -1");

        _repo.DeleteFighter(_queuedForDeletionFighterId);

        ReloadFighterList();
        foreach (var tab in FighterTabsNode.GetChildren().Cast<FighterEditor>())
        {
            if (tab.FighterId != _queuedForDeletionFighterId) continue;
            tab.QueueFree();
        }

        _queuedForDeletionFighterId = -1;
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
            DeckId = DeckId,
            IsRanged = false,
            IsSidekick = false,
            IsSmall = false,
            MaxHealth = 10,
            StartingHealth = 10,
            MeleeRange = 1,
            Movement = 2,
            Text = "",
            ImagePath = null,
            ScriptId = -1,
        };

        _repo.InsertFighter(fighter);
        var inserted = _repo.GetFighter(fighter.Name, DeckId);
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

    public void OnCreateCardButtonPressed()
    {
        NewCardWindow.SetEditData(
            _repo.GetCardNames(DeckId)
        );
        NewCardWindow.Show();
    }

    private int _queuedForDeletionCardId = -1;
    public void OnDeleteCardButtonPressed()
    {
        var selected = CardListNode.GetSelectedItems();
        if (selected.Length != 1) return;

        var cardId = CardListNode.GetItemMetadata(selected[0]).AsInt32();
        _queuedForDeletionCardId = cardId;
        var Card = _repo.GetCard(cardId);
        DeleteCardDialogNode.DialogText = $"Are you sure you want to delete card {Card.Name}?";
        DeleteCardDialogNode.Show();
    }

    public void OnDeleteCardDialogConfirmed()
    {
        if (_queuedForDeletionCardId == -1) throw new Exception($"{nameof(OnDeleteCardDialogConfirmed)} was called with {nameof(_queuedForDeletionCardId)} = -1");

        _repo.DeleteCard(_queuedForDeletionCardId);

        ReloadCardList();
        foreach (var tab in CardTabsNode.GetChildren().Cast<CardEditor>())
        {
            if (tab.CardId != _queuedForDeletionCardId) continue;
            tab.QueueFree();
        }

        _queuedForDeletionCardId = -1;
    }

    public void OnNewCardWindowCancelRequest()
    {
        NewCardWindow.Hide();
    }

    public void OnNewCardWindowConfirmRequest(string cardName)
    {
        NewCardWindow.Hide();

        var card = new CardModel()
        {
            Id = -1,
            AllowedFighters = "",
            Boost = 0,
            Count = 1,
            DeckId = DeckId,
            ImagePath = null,
            Labels = "",
            Name = cardName,
            StartingHandCount = 0,
            Text = "",
            Title = cardName,
            Type = CardModelType.Versatile,
            Value = 0,
            ScriptId = -1,
        };

        _repo.InsertCard(card);
        var inserted = _repo.GetCard(card.Name, DeckId);
        OpenCard(inserted.Id);
        ReloadCardList();
    }

    public void OnCardListItemActivated(int idx)
    {
        var cardId = CardListNode.GetItemMetadata(idx).AsInt32();

        OpenCard(cardId);
    }

    public void OnCardTabsTabBarTabClosePressed(long tabIdx)
    {
        CardTabsNode.RemoveChild(CardTabsNode.GetChild((int)tabIdx));
    }

    public void OnCollapseFighterListButtonPressed()
    {
        var state = FighterListContainer.Visible;
		CollapseFighterListButton.Text = state ? "<" : ">";
		FighterListContainer.Visible = !state; 
    }

    public void OnCollapseCardListButtonPressed()
    {
        var state = CardListContainer.Visible;
		CollapseCardListButton.Text = state ? "<" : ">";
		CardListContainer.Visible = !state; 
    }

    #endregion
}
