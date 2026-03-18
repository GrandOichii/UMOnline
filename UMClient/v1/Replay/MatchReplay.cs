using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UMCore.Matches;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Controllers;
using UMCore.Matches.Tokens;
using UMCore.Templates;
using UMDTO;

public class MatchStateRecorderPlayerControllerWrapper : PlayerControllerWrapper
{
    private List<Match.Data> _frames = [];
    public MatchStateRecorderPlayerControllerWrapper(IPlayerController controller) : base(controller)
    {
    }

    public override Task HandleActionChoice(string choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleAttackChoice(AvailableAttack choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleCardChoice(MatchCard choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleCardOrNothingChoice(MatchCard choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleFighterChoice(Fighter choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleNodeChoice(MapNode choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandlePathChoice(Path choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandlePlayerChoice(Player choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleStringChoice(string choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleTokenChoice(PlacedToken choice)
    {
        return Task.CompletedTask;
    }

    public override async Task HandleUpdate(Player player)
    {
        var frame = player.Match.GetData(player);
        _frames.Add(new()
        {
            Combat = frame.Combat,
            CurPlayerIdx = frame.CurPlayerIdx,
            Map = frame.Map,
            Players = [ .. player.Match.Players.Select(p => p.GetData(p))]
        });
    }
}

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

    public async Task LoadMatchRecord(
        LocalRepository repo,
        MatchRecordGet record
    )
    {
        var config = new MatchConfig()
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
        };
        var match = new Match(
            config,
            MapTemplate.GetBaskervilleTemplate(),
            repo.GetCore().Text
        )
        {
            Logger = null
        };

        MatchStateRecorderPlayerControllerWrapper recorder = null;
        var players = new QueuedPlayerCollection(config);

        Dictionary<string, IPlayerController> controllers = [];

        foreach (var player in record.Players)
        {
            IPlayerController controller = new ReplayerPlayerController(player.Responses);
            if (recorder is null)
            {
                recorder = new MatchStateRecorderPlayerControllerWrapper(controller);
                controller = recorder;
            }

            players.AddPlayer(
                player.Name,
                player.TeamIdx,
                repo.GetLoadoutTemplate(repo.GetDeck(player.Loadout).Id)
            );
            controllers.Add(player.Name, controller);
        }
        var cantRunReason = players.CanRun();
        if (!string.IsNullOrEmpty(cantRunReason))
        {
            throw new Exception($"Failed to add a player for replay, not enough checks: {cantRunReason}");
        }

        await match.AddPlayers(players, controllers);

        await match.Run();
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
