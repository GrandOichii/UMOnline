using Godot;
using System;
using System.Threading.Tasks;
using UMCore.Matches;
using UMCore.Matches.Players.Controllers;
using UMCore.Templates;
using UMDTO;

public partial class MatchReplayWindow : Window
{
    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public MatchReplay ReplayNode { get; set; }

    #endregion

    public async void LoadMatchRecord(
        LocalRepository repo,
        MatchRecordGet record
    )
    {
        await ReplayNode.LoadMatchRecord(repo, record);
    }

}
