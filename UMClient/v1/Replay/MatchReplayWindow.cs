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
        ReplayNode.LoadMatchRecord(record);

        var match = new Match(
            new()
            {
                Seed = record.Seed,
                RandomMatch = false,
                ActionsPerTurn = record.Config.ActionsPerTurn,
                ExhaustDamage = record.Config.ExhaustDamage,
                FirstPlayerIdx = record.Config.FirstPlayerIdx,
                InitialHandSize = record.Config.InitialHandSize,
                ManoeuvreDrawAmount = record.Config.ManoeuvreDrawAmount,
                MaxHandSize = record.Config.MaxHandSize,
                RandomFirstPlayer = record.Config.RandomFirstPlayer,
                TeamCount = record.Config.TeamCount,
                TeamSize = record.Config.TeamSize
            },
            MapTemplate.GetBaskervilleTemplate(),
            repo.GetCore().Text
        )
        {
            Logger = null
        };

        foreach (var player in record.Players)
        {
            await match.AddPlayer(
                player.Name,
                player.TeamIdx,
                repo.GetLoadoutTemplate(repo.GetDeck(player.Loadout).Id),
                new ReplayerPlayerController(player.Responses)
            );
        }

        await match.Run();
        // TODO launch match
    }

}
