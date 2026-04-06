using System;
using System.Collections.Generic;
using Godot;

public partial class FighterEditor : TabContainer
{
    #region Signals

    [Signal]
    public delegate void FighterChangedEventHandler(int fighterId);
    [Signal]
    public delegate void FighterNameChangedEventHandler(int fighterId);
    [Signal]
    public delegate void FighterImageImportRequestEventHandler(int fighterId, string path);
    [Signal]
    public delegate void FighterScriptChangedEventHandler(int fighterId);

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
    [Export]
    public NameEditWindow RenameWindowNode { get; set; }
    [Export]
    public ScriptEditor ScriptEditorNode { get; set; }
    [Export]
	public AcceptDialog ImageFailureDialogNode { get; set; }

    #endregion

    public int FighterId { get; private set; }
    private int _deckId;
    private string _imagePath;
    private int _scriptId;
    private bool _editable;
    private Func<List<string>> _fighterNamesGetter;

    public void SetEssentials(
        Func<List<string>> fighterNamesGetter
    )
    {
        _fighterNamesGetter = fighterNamesGetter;
    }

    public int GetFighterId() => FighterId;

    // public override void _Ready()
    // {
    // }

    public ScriptModel BuildScriptModel() => ScriptEditorNode.BuildScriptModel();

    public FighterModel BuildFighterModel()
    {
        return new()
        {
            Id = FighterId,
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
            ScriptId = _scriptId,
        };
    }

    public void LoadFighter(FighterModel fighter, ScriptModel script, bool editable)
    {
        FighterId = fighter.Id;
        _deckId = fighter.DeckId;
        _scriptId = fighter.ScriptId;
        _editable = editable;

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

		SetIsEditable(editable);
		LoadFighterImage();

        ScriptEditorNode.LoadScriptModel(script, editable);
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
		if (err != Error.Ok)
        {
            ImageFailureDialogNode.DialogText = "Failed to load image!";
            ImageFailureDialogNode.Show();
            return;
        }
        

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
        if (err != Error.Ok)
        {
            ImageFailureDialogNode.DialogText = "Failed to import image!";
            ImageFailureDialogNode.Show();
            return;
        }
        

        var texture = ImageTexture.CreateFromImage(image);
        FighterImageNode.Texture = texture;

        EmitSignal(SignalName.FighterImageImportRequest, FighterId, path);
    }

    public void OnRenameButtonPressed()
    {
        RenameWindowNode.SetEditData(
			NameEditNode.Text,
			_fighterNamesGetter()
		);

		RenameWindowNode.Show();
    }

    public void OnRenameWindowCancelRequest()
    {
        RenameWindowNode.Hide();
    }

    public void OnRenameWindowConfirmRequest(string newName)
    {
        RenameWindowNode.Hide();
        NameEditNode.Text = newName;
		EmitSignal(SignalName.FighterChanged, FighterId);
		EmitSignal(SignalName.FighterNameChanged, FighterId);
    }

    public void OnScriptScriptModelChanged()
    {
        EmitSignal(SignalName.FighterScriptChanged, FighterId);
    }

    #region FighterChanged emitters

    public void OnNameEditTextChanged(string _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnAmountValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnIsSidekickCheckPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnIsSmallFighterCheckPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnMaxHealthValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnStartingHealthValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnMeleeRangeValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnIsRangedCheckPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnMovementValueChanged(int _)
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnCanMoveOverOpposingPressed()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    public void OnTextTextChanged()
    {
        if (!_editable) return;
        EmitSignal(SignalName.FighterChanged, FighterId);
    }

    #endregion

    #endregion
}
