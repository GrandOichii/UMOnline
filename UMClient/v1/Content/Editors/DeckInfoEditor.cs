using Godot;
using System;
using System.IO;
using UMCore.Matches;

public partial class DeckInfoEditor : MarginContainer
{
	#region Signals

	[Signal]
	public delegate void DeckInfoChangedEventHandler();
	[Signal]
	public delegate void DeckNameChangedEventHandler();
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
	public CheckBox HasStartingHandSizeNode { get; set; }
	[Export]
	public SpinBox StartingHandSizeNode { get; set; }
	[Export]
	public CheckBox HasMaxHandSizeNode { get; set; }
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
	[Export]
	public AcceptDialog ImageFailureDialogNode { get; set; }

	#endregion

	private LocalRepository _repo;
	private string _cardBackPath;
	private int _deckId;
	private bool _editable;

	public void SetEssentials(
		LocalRepository repo
	)
	{
		_repo = repo;
	}

	public DeckModel BuildDeckModel() => new()
	{
		Name = NameEditNode.Text,
		ChoosesSidekick = ChoosesSidekickCheckNode.ButtonPressed,
		StartsWithSidekicks = StartsWithSidekicksCheckNode.ButtonPressed,
		StartingHandSize = HasStartingHandSizeNode.ButtonPressed ? (int)StartingHandSizeNode.Value : null,
		MaxHandSize = HasMaxHandSizeNode.ButtonPressed ? (int)MaxHandSizeNode.Value : null,
		Editable = true,
		Id = _deckId,
		Description = DescriptionEditNode.Text,
		CardBackPath = _cardBackPath,
	};

	public void LoadDeck(DeckModel deck)
	{
		_deckId = deck.Id;
		_editable = deck.Editable;
		NameEditNode.Text = deck.Name;
		ChoosesSidekickCheckNode.ButtonPressed = deck.ChoosesSidekick;
		StartsWithSidekicksCheckNode.ButtonPressed = deck.ChoosesSidekick;
		HasStartingHandSizeNode.ButtonPressed = deck.StartingHandSize is null;
		StartingHandSizeNode.Value = deck.StartingHandSize is null 
			? MatchConfig.Default1x1.InitialHandSize 
			: (double)deck.StartingHandSize;
		HasMaxHandSizeNode.ButtonPressed = deck.MaxHandSize is null;
		MaxHandSizeNode.Value = deck.MaxHandSize is null
			? MatchConfig.Default1x1.MaxHandSize
			: (double)deck.MaxHandSize;
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
		HasMaxHandSizeNode.Disabled = !value;
		HasStartingHandSizeNode.Disabled = !value;
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
		if (err != Error.Ok)
        {
            ImageFailureDialogNode.DialogText = "Failed to import image!";
            ImageFailureDialogNode.Show();
            return;
        }
        
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
		EmitSignal(SignalName.DeckNameChanged);
	}

	#region FighterChanged emitters

	public void OnNameTextChanged(string _)
	{
		if (!_editable) return;
		EmitSignal(SignalName.DeckInfoChanged);
	}

	public void OnChoosesSidekickCheckPressed()
	{
		if (!_editable) return;
		EmitSignal(SignalName.DeckInfoChanged);
	}

	public void OnStartsWithSidekicksCheckPressed()
	{
		if (!_editable) return;
		EmitSignal(SignalName.DeckInfoChanged);
	}

	public void OnStartingHandSizeValueChanged(int _)
	{
		if (!_editable) return;
		EmitSignal(SignalName.DeckInfoChanged);
	}

	public void OnMaxHandSizeValueChanged(int _)
	{
		if (!_editable) return;
		EmitSignal(SignalName.DeckInfoChanged);
	}

	public void OnDescriptionTextChanged()
	{
		if (!_editable) return;
		EmitSignal(SignalName.DeckInfoChanged);
	}

	#endregion

	#endregion
}
