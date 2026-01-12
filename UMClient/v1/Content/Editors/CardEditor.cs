using Godot;
using System;

public partial class CardEditor : TabContainer
{
	#region Signals

	[Signal]
	public delegate void CardChangedEventHandler();
	[Signal]
	public delegate void CardImageImportRequestEventHandler();

	#endregion

	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public LineEdit TitleEditNode { get; set; }
	[Export]
	public SpinBox DeckCountNode { get; set; }
	[Export]
	public SpinBox StartingHandCount { get; set; }
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

	#endregion

	public override void _Ready()
	{
		// TODO remove
		Connect(SignalName.CardChanged, Callable.From(() =>
		{
			GD.Print("Changed");
		}));

		Connect(SignalName.CardImageImportRequest, Callable.From((string p) =>
		{
			GD.Print($"Changed import path to {p}");
		}));
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
        CardImageNode.Texture = texture;

        EmitSignal(SignalName.CardImageImportRequest, path);
    }

    public void OnTypeOptionItemSelected(int idx)
    {
        EmitSignal(SignalName.CardChanged);

        ValueNode.Editable = idx != 0; // scheme
    }
    
    public void OnBoostCheckToggled(bool hasBoost)
    {
        EmitSignal(SignalName.CardChanged);

        BoostValueNode.Editable = hasBoost;
    }

    #region FighterChanged emitters

    public void OnTitleEditTextChanged(string _)
    {
        EmitSignal(SignalName.CardChanged);
    }
    
    public void OnDeckCountValueChanged(int _)
    {
        EmitSignal(SignalName.CardChanged);
    }
    
    public void OnStartingHandCountValueChanged(int _)
    {
        EmitSignal(SignalName.CardChanged);
    }
    
    public void OnValueValueChanged(int _)
    {
        EmitSignal(SignalName.CardChanged);
    }
    
    public void OnBoostValueValueChanged(int _)
    {
        EmitSignal(SignalName.CardChanged);
    }
    
    public void OnTextTextChanged()
    {
        EmitSignal(SignalName.CardChanged);
    }
    
    #endregion

    #endregion
}
