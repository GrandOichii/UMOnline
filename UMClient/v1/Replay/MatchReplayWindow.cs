using Godot;
using System;
using UMDTO;

public partial class MatchReplayWindow : Window
{
    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public MatchReplay ReplayNode { get; set; }

    #endregion

    public void LoadMatchRecord(MatchRecordGet record)
    {
        GD.Print(record.Players.Count);
    }
}
