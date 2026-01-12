using Godot;
using UMClient.Storage.Models;

public partial class FighterEditor : TabContainer
{
    #region Signals

    [Signal]
    public delegate void FighterChangedEventHandler();
    [Signal]
    public delegate void FighterImageImportRequestEventHandler();

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
	public CheckBox IsRangedCheck { get; set; }
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
    public TagsEditor AllowedFightersEditor { get; set; }
    [Export]
    public TagsEditor LabelsEditor { get; set; }

    #endregion

    public override void _Ready()
    {
        // TODO remove
        Connect(SignalName.FighterChanged, Callable.From(() =>
        {
            GD.Print("Changed");
        }));

        Connect(SignalName.FighterImageImportRequest, Callable.From((string p) =>
        {
            GD.Print($"Changed import path to {p}");
        }));
    }


    public FighterModel GetFighterModel()
    {
        return new()
        {
            Id = -1, // TODO
            DeckId = -1, // TODO  
            Name = NameEditNode.Text,
            // TODO
        };
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

        EmitSignal(SignalName.FighterImageImportRequest, path);
    }

    #region FighterChanged emitters

    public void OnNameEditTextChanged(string _)
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnAmountValueChanged(int _)
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnIsSidekickCheckPressed()
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnIsSmallFighterCheckPressed()
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnMaxHealthValueChanged(int _)
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnStartingHealthValueChanged(int _)
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnMeleeRangeValueChanged(int _)
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnIsRangedCheckPressed()
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnMovementValueChanged(int _)
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnCanMoveOverOpposingPressed()
    {
        EmitSignal(SignalName.FighterChanged);
    }

    public void OnTextTextChanged()
    {
        EmitSignal(SignalName.FighterChanged);
    }

    #endregion

    #endregion
}
