using Godot;
using System;

public partial class MatchReplay : Control
{
    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public Control OverlayNode { get; set; }
    [Export]
    public Control MatchDisplayNode { get; set; }

    #endregion

    public override void _Ready()
    {
        OverlayNode.Show();
    }
    
    #region Signal connections

    public void OnPrevStateButtonPressed()
    {
        // TODO
        GD.Print("OnPrevStateButtonPressed");
    }

    public void OnNextStateButtonPressed()
    {
        // TODO
        GD.Print("OnNextStateButtonPressed");
    }

    #endregion
}
