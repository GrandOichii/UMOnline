using Godot;
using UMCore.Matches.Players.Cards;

public partial class FighterEditor : TabContainer
{
    #region Signals

    [Signal]
    public delegate void FighterChangedEventHandler(int fighterId);
    [Signal]
    public delegate void FighterNameChangedEventHandler(int fighterId);
    [Signal]
    public delegate void FighterImageImportRequestEventHandler(int fighterId, string path);

    #endregion

    #region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public LineEdit NameEditNode { get; set; }
	[Export]
	public SpinBox AmountNode { get; set; }
	[Export]
	public CheckBox IsSidekickCheckNode { get; set; }
	[Export]
	public CheckBox IsSmallFighterCheckNode { get; set; }
	[Export]
	public SpinBox MaxHealthNode { get; set; }
	[Export]
	public SpinBox StartingHealthNode { get; set; }
	[Export]
	public SpinBox MeleeRangeNode { get; set; }
	[Export]
	public CheckBox IsRangedCheckNode { get; set; }
	[Export]
	public SpinBox MovementNode { get; set; }
	[Export]
	public CheckBox CanMoveOverOpposingNode { get; set; }
	[Export]
	public TextEdit TextNode { get; set; }
	[Export]
	public TextureRect FighterImageNode { get; set; }
    [Export]
    public ImageImporter ImageImporterNode { get; set; }
    [Export]
    public Button RenameButton { get; set; }
    [Export]
    public Button ImportImageButton { get; set; }

    #endregion

    private int _fighterId;
    private int _deckId;
    private string _imagePath;
    private bool _editable;

    public int GetFighterId() => _fighterId;

    // public override void _Ready()
    // {
    // }

    public FighterModel BuildFighterModel()
    {
        return new()
        {
            Id = _fighterId,
            DeckId = _deckId,
            Name = NameEditNode.Text,
            Amount = (int)AmountNode.Value,
            CanMoveOverOpposing = CanMoveOverOpposingNode.ButtonPressed,
            IsRanged = IsRangedCheckNode.ButtonPressed,
            IsSidekick = IsSidekickCheckNode.ButtonPressed,
            IsSmall = IsSmallFighterCheckNode.ButtonPressed,
            MaxHealth = (int)MaxHealthNode.Value,
            MeleeRange = (int)MeleeRangeNode.Value,
            Movement = (int)MovementNode.Value,
            StartingHealth = (int)StartingHealthNode.Value,
            Text = TextNode.Text,
            ImagePath = _imagePath,
        };
    }

    public void LoadFighter(FighterModel fighter, DeckModel deck)
    {
        _fighterId = fighter.Id;
        _deckId = fighter.DeckId;
        _editable = deck.Editable;

        NameEditNode.Text = fighter.Name;
        AmountNode.Value = fighter.Amount;
        CanMoveOverOpposingNode.ButtonPressed = fighter.CanMoveOverOpposing;
        IsRangedCheckNode.ButtonPressed = fighter.IsRanged;
        IsSidekickCheckNode.ButtonPressed = fighter.IsSidekick;
        IsSmallFighterCheckNode.ButtonPressed = fighter.IsSmall;
        MaxHealthNode.Value = fighter.MaxHealth;
        StartingHealthNode.Value = fighter.StartingHealth;
        TextNode.Text = fighter.Text;
        MovementNode.Value = fighter.Movement;
        MeleeRangeNode.Value = fighter.MeleeRange;
        _imagePath = fighter.ImagePath;

		SetIsEditable(deck.Editable);
		LoadFighterImage();
    }

    private void SetIsEditable(bool value)
	{
		RenameButton.Disabled = !value;
		AmountNode.Editable = value;
		CanMoveOverOpposingNode.Disabled = !value;
		IsRangedCheckNode.Disabled = !value;
		IsSidekickCheckNode.Disabled = !value;
		IsSmallFighterCheckNode.Disabled = !value;
		MaxHealthNode.Editable = value;
		StartingHealthNode.Editable = value;
		TextNode.Editable = value;
        MeleeRangeNode.Editable = value;
        MovementNode.Editable = value;
        ImportImageButton.Disabled = !value;
	}

    public void LoadFighterImage()
	{
		FighterImageNode.Texture = null;
		if (_imagePath is null) return;

		var image = new Image();

		var err = image.Load(_imagePath);
		// TODO handle
		var texture = ImageTexture.CreateFromImage(image);
		FighterImageNode.Texture = texture;
	}

    public void UpdateFighterImage(string path)
	{
		_imagePath = path;

		LoadFighterImage();
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
        // TODO handle
        var texture = ImageTexture.CreateFromImage(image);
        FighterImageNode.Texture = texture;

        EmitSignal(SignalName.FighterImageImportRequest, _fighterId, path);
    }

    #region FighterChanged emitters

    public void OnNameEditTextChanged(string _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnAmountValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnIsSidekickCheckPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnIsSmallFighterCheckPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnMaxHealthValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnStartingHealthValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnMeleeRangeValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnIsRangedCheckPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnMovementValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnCanMoveOverOpposingPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    public void OnTextTextChanged()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, _fighterId);
    }

    #endregion

    #endregion
}
