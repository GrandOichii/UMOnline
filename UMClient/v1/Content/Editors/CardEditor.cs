using Godot;
using System;
using System.Collections.Generic;
using UMCore.Matches.Players.Cards;

public partial class CardEditor : TabContainer
{
	#region Signals

	[Signal]
	public delegate void CardChangedEventHandler(int cardId);
	[Signal]
	public delegate void CardNameChangedEventHandler(int cardId);
	[Signal]
	public delegate void CardImageImportRequestEventHandler(int cardId, string path);
    [Signal]
    public delegate void CardScriptChangedEventHandler(int cardId);

	#endregion

	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public LineEdit NameEditNode { get; set; }
	[Export]
	public LineEdit TitleEditNode { get; set; }
	[Export]
	public SpinBox DeckCountNode { get; set; }
	[Export]
	public SpinBox StartingHandCountNode { get; set; }
	[Export]
	public OptionButton TypeOptionNode { get; set; }
	[Export]
	public SpinBox ValueNode { get; set; }
	[Export]
	public CheckBox BoostCheckNode { get; set; }
	[Export]
	public SpinBox BoostValueNode { get; set; }
	[Export]
	public TextEdit TextNode { get; set; }
    [Export]
    public TextureRect CardImageNode { get; set; }
    [Export]
    public ImageImporter ImageImporterNode { get; set; }
    [Export]
    public TagsEditor AllowedFightersTagsNode { get; set; }
    [Export]
    public TagsEditor LabelsTagsNode { get; set; }
    [Export]
    public Button RenameButton { get; set; }
    [Export]
    public Button ImportImageButton { get; set; }
    [Export]
    public ScriptEditor ScriptEditorNode { get; set; }
    [Export]
    public NameEditWindow RenameWindowNode { get; set; }
    [Export]
    public AcceptDialog ImageFailureDialogNode { get; set; }

	#endregion

    public int CardId { get; private set; }
    private int _deckId;
    private string _imagePath;
    private int _scriptId;
    private bool _editable;
    private Func<List<string>> _cardNamesGetter;

    public int GetCardId() => CardId;

    public void SetEssentials(
        Func<List<string>> cardNamesGetter
    )
    {
        _cardNamesGetter = cardNamesGetter;
    }

	public override void _Ready()
	{
	}

    public void LoadCard(CardModel card, ScriptModel script, bool editable)
    {
        CardId = card.Id;
        _deckId = card.DeckId;
        _scriptId = card.ScriptId;
        _editable = editable;

        NameEditNode.Text = card.Name;
        TitleEditNode.Text = card.Title;
        DeckCountNode.Value = card.Count;
        StartingHandCountNode.Value = card.StartingHandCount;
        AllowedFightersTagsNode.LoadTags(card.GetAllowedFighters());
        LabelsTagsNode.LoadTags(card.GetLabels());
        TypeOptionNode.Select((int)card.Type);
        ValueNode.Value = card.Value;

        var hasBoost = card.Boost >= 0;
        BoostCheckNode.ButtonPressed = hasBoost;
        BoostValueNode.Value = hasBoost ? card.Boost : -1;

        TextNode.Text = card.Text;
        _imagePath = card.ImagePath;

        SetIsEditable(editable);
		LoadCardImage();

        OnBoostCheckToggled(hasBoost);
        OnTypeOptionItemSelected((int)card.Type);

        ScriptEditorNode.LoadScriptModel(script, editable);
    }

    private void SetIsEditable(bool value)
	{
		RenameButton.Disabled = !value;
        TitleEditNode.Editable = value;
        DeckCountNode.Editable = value;
        StartingHandCountNode.Editable = value;
        AllowedFightersTagsNode.SetEditable(value);
        LabelsTagsNode.SetEditable(value);
        TypeOptionNode.Disabled = !value;
        ValueNode.Editable = value;
        BoostCheckNode.Disabled = !value;
        BoostValueNode.Editable = value;
        TextNode.Editable = value;
        ImportImageButton.Disabled = !value;
	}

    public ScriptModel BuildScriptModel() => ScriptEditorNode.BuildScriptModel();

    public CardModel BuildCardModel() => new()
    {
        Id = CardId,
        DeckId = _deckId,
        AllowedFighters = CardModel.ToAllowedFighters(AllowedFightersTagsNode.GetTags()),
        Labels = CardModel.ToLabels(LabelsTagsNode.GetTags()),
        Boost = BoostCheckNode.ButtonPressed ? (int)BoostValueNode.Value : -1,
        Count = (int)DeckCountNode.Value,
        StartingHandCount = (int)StartingHandCountNode.Value,
        Name = NameEditNode.Text,
        Text = TextNode.Text,
        Title = TitleEditNode.Text,
        Type = (CardModelType)TypeOptionNode.Selected,
        Value = (int)ValueNode.Value,
        ImagePath = _imagePath,
        ScriptId = _scriptId,
    };

    public void UpdateCardImage(string path)
    {
        _imagePath = path;

		LoadCardImage();
    }

    public void LoadCardImage()
	{
		CardImageNode.Texture = null;
		if (_imagePath is null) return;

		var image = new Image();

		var err = image.Load(_imagePath);
        if (err != Error.Ok)
        {
            ImageFailureDialogNode.DialogText = "Failed to load image!";
            ImageFailureDialogNode.Show();
            return;
        }
		var texture = ImageTexture.CreateFromImage(image);
		CardImageNode.Texture = texture;
	}

    #region Signal connections

    public void OnImportImageButtonPressed()
    {
        ImageImporterNode.Show();
    }

    public void OnImageImporterFileSelected(string path)
    {
        var image = new Image();

        var err = image.Load(path);
        if (err != Error.Ok)
        {
            ImageFailureDialogNode.DialogText = "Failed to import image!";
            ImageFailureDialogNode.Show();
            return;
        }
        
        var texture = ImageTexture.CreateFromImage(image);
        CardImageNode.Texture = texture;

        EmitSignal(SignalName.CardImageImportRequest, CardId, path);
    }

    public void OnTypeOptionItemSelected(int idx)
    {
        ValueNode.Editable = idx != 0; // scheme

        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }
    
    public void OnBoostCheckToggled(bool hasBoost)
    {
        BoostValueNode.Editable = hasBoost;

        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }

    public void OnRenameWindowCancelRequest()
    {
        RenameWindowNode.Hide();
    }

    public void OnRenameWindowConfirmRequest(string newName)
    {
        RenameWindowNode.Hide();
        NameEditNode.Text = newName;
		EmitSignal(SignalName.CardChanged, CardId);
		EmitSignal(SignalName.CardNameChanged, CardId);
    }

    public void OnRenameButtonPressed()
    {
        RenameWindowNode.SetEditData(
			NameEditNode.Text,
			_cardNamesGetter()
		);

		RenameWindowNode.Show();
    }

    public void OnScriptScriptModelChanged()
    {
        EmitSignal(SignalName.CardScriptChanged, CardId);
    }

    #region CardChanged emitters

    public void OnTitleEditTextChanged(string _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }
    
    public void OnDeckCountValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }
    
    public void OnStartingHandCountValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }
    
    public void OnValueValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }
    
    public void OnBoostValueValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }
    
    public void OnTextTextChanged()
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }

    public void OnAllowedFightersEditorTagsChanged(Godot.Collections.Array<string> _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }

    public void OnLabelsEditorTagsChanged(Godot.Collections.Array<string> _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.CardChanged, CardId);
    }
    
    #endregion

    #endregion
}
