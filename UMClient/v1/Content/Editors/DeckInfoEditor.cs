using Godot;
using System;

public partial class DeckInfoEditor : MarginContainer
{
	[Export]
	public DeckEditor Parent { get; set; }  
	
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
	public TextEdit DescriptionEditNode { get; set; }
	[Export]
	public TextureRect CardBackNode { get; set; }
	[Export]
	public ImageImporter CardBackImporter { get; set; }
	[Export]
	public NameEditWindow RenameWindow { get; set; }
	[Export]
	public Button RenameButton { get; set; }

	#endregion

	private LocalRepository _repo;

	public void SetEssentials(
		LocalRepository repo
	)
	{
		_repo = repo;
	}

	public DeckModel GetDeck() => new()
	{
		Name = NameEditNode.Text,
		ChoosesSidekick = ChoosesSidekickCheckNode.ButtonPressed,
		StartsWithSidekicks = StartsWithSidekicksCheckNode.ButtonPressed,
		StartingHandSize = (int)StartingHandSizeNode.Value,
		MaxHandSize = (int)MaxHandSizeNode.Value,
		Editable = true,
		Id = -1,
		Description = DescriptionEditNode.Text,
	};

	public void LoadDeck(DeckModel deck)
	{
		NameEditNode.Text = deck.Name;
		ChoosesSidekickCheckNode.ButtonPressed = deck.ChoosesSidekick;
		StartsWithSidekicksCheckNode.ButtonPressed = deck.ChoosesSidekick;
		StartingHandSizeNode.Value = (double)deck.StartingHandSize;
		MaxHandSizeNode.Value = (double)deck.MaxHandSize;
		DescriptionEditNode.Text = deck.Description;

		SetIsEditable(deck.Editable);
		// TODO CardBackNode
	}

	private void SetIsEditable(bool value)
	{
		RenameButton.Disabled = !value;
		ChoosesSidekickCheckNode.Disabled = !value;
		StartsWithSidekicksCheckNode.Disabled = !value;
		StartingHandSizeNode.Editable = value;
		MaxHandSizeNode.Editable = value;
	}

	public override void _Ready()
	{
		base._Ready();

		// TODO remove
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

	public void OnRenameButtonPressed()
	{
		RenameWindow.SetEditData(
			NameEditNode.Text,
			_repo.GetDeckNames()
		);

		RenameWindow.Show();
	}

	public void OnRenameWindowCancelRequest()
	{
		RenameWindow.Hide();
	}

	public void OnRenameWindowConfirmRequest(string newName)
	{
		RenameWindow.Hide();
		NameEditNode.Text = newName;
		EmitSignal(SignalName.DeckInfoChanged);

		// TODO update deck lists and deck tab names
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

	public void OnDescriptionTextChanged()
	{
		EmitSignal(SignalName.DeckInfoChanged);
	}

	#endregion

	#endregion
}
