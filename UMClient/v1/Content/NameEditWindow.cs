using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

public partial class NameEditWindow : Window
{
    [Export]
    public Color GoodNameColor { get; set; }
    [Export]
    public Color BadNameColor { get; set; }
    [Export]
    public string GoodNameLabel { get; set; }
    [Export]
    public string BadNameLabel { get; set; }
    [Export]
    public string TakenNameLabel { get; set; }
    [Export]
    public string EmptyNameLabel { get; set; }
    [Export]
    public string ConfirmButtonText { get; set; }

    #region Signals

    [Signal]
    public delegate void ConfirmRequestEventHandler(string newName);
    [Signal]
    public delegate void CancelRequestEventHandler();

    #endregion

    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public LineEdit NameEditNode { get; set; }
    [Export]
    public Label TipNode { get; set; }
    [Export]
    public Button ConfirmButtonNode { get; set; }

    #endregion

    private string _editedName;
    private List<string> _takenNames;

    public override void _Ready()
    {
        ConfirmButtonNode.Text = ConfirmButtonText;
    }

    public void SetEditData(List<string> takenNames)
    {
        SetEditData(null, takenNames);
    }

    public void SetEditData(string name, List<string> takenNames)
    {
        _editedName = name;
        _takenNames = takenNames;
        var displayedName = name ?? "";
        NameEditNode.Text = displayedName;
        OnNameTextChanged(displayedName);
    }

    private void SetTipColor(Color c)
    {
        TipNode.Set("theme_override_colors/font_color", c);
    }

    #region Signal connections

    public void OnNameTextChanged(string newText)
    {
        SetTipColor(BadNameColor);
        ConfirmButtonNode.Disabled = true;

        if (newText.Length == 0)
        {
            TipNode.Text = EmptyNameLabel;
            return;
        }

        var count = _takenNames.Count(n => n == newText);
        if (_editedName == newText) --count;
        if (count > 0)
        {
            TipNode.Text = TakenNameLabel;
            return;
        }

        TipNode.Text = GoodNameLabel;
        SetTipColor(GoodNameColor);
        ConfirmButtonNode.Disabled = false;
    }

    public void OnConfirmButtonPressed()
    {
        EmitSignalConfirmRequest(NameEditNode.Text);
    }

    public void OnCancelButtonPressed()
    {
        EmitSignalCancelRequest();

    }

    #endregion
}