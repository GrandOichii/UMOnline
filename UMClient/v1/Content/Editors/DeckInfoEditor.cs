using Godot;
using System;
using System.IO;

public partial class DeckInfoEditor : MarginContainer
{
	#region Signals

	[Signal]
	public delegate void DeckInfoChangedEventHandler();
	[Signal]
	public delegate void CardBackImportRequestEventHandler(string path);

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
	[Export]
	public Button CardBackImportButton { get; set; }

	#endregion

	private LocalRepository _repo;
	private string _cardBackPath;

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
		CardBackPath = _cardBackPath,
	};

	public void LoadDeck(DeckModel deck)
	{
		NameEditNode.Text = deck.Name;
		ChoosesSidekickCheckNode.ButtonPressed = deck.ChoosesSidekick;
		StartsWithSidekicksCheckNode.ButtonPressed = deck.ChoosesSidekick;
		StartingHandSizeNode.Value = (double)deck.StartingHandSize;
		MaxHandSizeNode.Value = (double)deck.MaxHandSize;
		DescriptionEditNode.Text = deck.Description;
		_cardBackPath = deck.CardBackPath;

		SetIsEditable(deck.Editable);
		LoadCardBack();
	}

	public void UpdateCardBack(string path)
	{
		_cardBackPath = path;

		LoadCardBack();
	}

	private void SetIsEditable(bool value)
	{
		RenameButton.Disabled = !value;
		CardBackImportButton.Disabled = !value;
		ChoosesSidekickCheckNode.Disabled = !value;
		StartsWithSidekicksCheckNode.Disabled = !value;
		StartingHandSizeNode.Editable = value;
		MaxHandSizeNode.Editable = value;
		DescriptionEditNode.Editable = value;
	}

	// public override void _Ready()
	// {
	// 	base._Ready();
	// }

	public void LoadCardBack()
	{
		CardBackNode.Texture = null;
		if (_cardBackPath is null) return;

		var image = new Image();

		var err = image.Load(_cardBackPath);
		// TODO handle
		var texture = ImageTexture.CreateFromImage(image);
		CardBackNode.Texture = texture;
	}

	#region Signal connections

	public void OnCardBackImportButtonPressed()
	{
		CardBackImporter.Show();
	}

	public void OnCardBackImporterFileSelected(string path)
	{
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
