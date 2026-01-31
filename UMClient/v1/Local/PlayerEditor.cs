using Godot;
using System;
using UMCore.Matches.Players;
using UMCore.Templates;

public partial class PlayerEditor : HBoxContainer, IPlayerEditor
{
    #region Nodes

    [Export]
    public LineEdit NameEditNode { get; set; }
    [Export]
    public OptionButton DeckOption { get; set; }
    [Export]
    public OptionButton TeamOption { get; set; }

    #endregion

    public LocalMatchesTab LMT { get; private set; }

    public PlayerEditorResult Build()
    {
        throw new NotImplementedException();
    }

    public void LoadLocalMatchesTab(LocalMatchesTab lmt)
    {
        LMT = lmt;
    }

    public void LoadName(string name)
    {
        NameEditNode.Text = name;
    }

}
