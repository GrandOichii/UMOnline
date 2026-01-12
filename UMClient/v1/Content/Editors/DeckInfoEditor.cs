using Godot;
using System;

public partial class DeckInfoEditor : MarginContainer
{
	#region Signals

	[Signal]
	public delegate void DeckInfoChangedEventHandler();
	[Signal]
	public delegate void CardBackImportRequestEventHandler();

	#endregion

	#region Nodes

	[ExportGroup("Nodes")]
	[Export]
	public LineEdit NameEditNode { get; set; }
	[Export]
	public CheckBox ChoosesSidekickCheckNode { get; set; }
	[Export]
	public CheckBox StartsWithSidekicksCheckNode { get; set; }
	[Export]
    public SpinBox StartingHandSizeNode { get; set; }
	[Export]
    public SpinBox MaxHandSizeNode { get; set; }
	[Export]
    public TextureRect CardBackNode { get; set; }
	[Export]
    public ImageImporter CardBackImporter { get; set; }

	#endregion

	public override void _Ready()
	{
		base._Ready();

		// TODO remove
		Connect(SignalName.DeckInfoChanged, Callable.From(() =>
		{
			GD.Print("Changed");
		}));

		Connect(SignalName.CardBackImportRequest, Callable.From((string p) =>
		{
			GD.Print($"Changed card back import path to {p}");
		}));
	}

	#region Signal connections

    public void OnCardBackImportButtonPressed()
    {
        CardBackImporter.Show();
    }

    public void OnCardBackImporterFileSelected(string path)
    {
        var image = new Image();

        var err = image.Load(path);
        // TODO handle
        var texture = ImageTexture.CreateFromImage(image);
        CardBackNode.Texture = texture;

        EmitSignal(SignalName.CardBackImportRequest, path);
    }

    #region FighterChanged emitters

    public void OnNameTextChanged(string _)
    {
        EmitSignal(SignalName.DeckInfoChanged);
    }

    public void OnChoosesSidekickCheckPressed()
    {
        EmitSignal(SignalName.DeckInfoChanged);
    }

    public void OnStartsWithSidekicksCheckPressed()
    {
        EmitSignal(SignalName.DeckInfoChanged);
    }

    public void OnStartingHandSizeValueChanged(int _)
    {
        EmitSignal(SignalName.DeckInfoChanged);
    }

    public void OnMaxHandSizeValueChanged(int _)
    {
        EmitSignal(SignalName.DeckInfoChanged);
    }

    #endregion

    #endregion
}
