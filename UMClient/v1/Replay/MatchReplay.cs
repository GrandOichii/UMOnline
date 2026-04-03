using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UMCore.Matches;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Controllers;
using UMCore.Matches.Tokens;
using UMCore.Templates;
using UMDTO;

public class MatchFrame
{
    public Match.Data Data { get; }

    public MatchFrame(Match.Data data)
    {
        Data = data;
    }

    public string GetHash()
    {
        return JsonSerializer.Serialize(Data);
    }
}

public class MatchStateRecorderPlayerControllerWrapper : PlayerControllerWrapper
{
    public List<MatchFrame> Frames { get; } = [];

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
        AddFrame(new()
        {
            Combat = frame.Combat,
            CurPlayerIdx = frame.CurPlayerIdx,
            Map = frame.Map,
            Players = [.. player.Match.Players.Select(p => p.GetData(p))]
        });
    }

    private void AddFrame(Match.Data data)
    {
        var frame = new MatchFrame(data);
        if (Frames.Count > 0)
        {
            var last = Frames.Last();
            if (last.GetHash() == frame.GetHash()) return;
        }
        Frames.Add(frame);
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

    private LocalRepository _repo;
	private MatchRecordGet _record;

    public override void _Ready()
    {
        OverlayNode.Show();
    }

    public void LoadMatchRecord(
        LocalRepository repo,
        MatchRecordGet record
    )
    {
        _repo = repo;
		_record = record;
        Task.Run(StartReplay);
    }
    private async Task StartReplay() {
        try
        {
            var config = new MatchConfig()
            {
                Seed = _record.Seed,
                RandomMatch = false,

                ActionsPerTurn = _record.Config.ActionsPerTurn,
                ExhaustDamage = _record.Config.ExhaustDamage,
                FirstPlayerIdx = _record.Config.FirstPlayerIdx,
                InitialHandSize = _record.Config.InitialHandSize,
                ManoeuvreDrawAmount = _record.Config.ManoeuvreDrawAmount,
                MaxHandSize = _record.Config.MaxHandSize,
                RandomFirstPlayer = _record.Config.RandomFirstPlayer,
                TeamCount = _record.Config.TeamCount,
                TeamSize = _record.Config.TeamSize
            };
            var match = new Match(
                config,
                MapTemplate.GetBaskervilleTemplate(),
                _repo.GetCore().Text
            )
            {
                Logger = new GDLogger()
            };

            MatchStateRecorderPlayerControllerWrapper recorder = null;
            var players = new QueuedPlayerCollection(config);

            Dictionary<string, IPlayerController> controllers = [];

            foreach (var player in _record.Players)
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
                    _repo.GetLoadoutTemplate(_repo.GetDeck(player.Loadout).Id)
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
            Callable.From(() =>
            {
                GD.Print($"Generated {recorder.Frames.Count} frames");
            }).CallDeferred();

        }
        catch (Exception e)
        {
            Callable.From(() =>
            {
                var exception = e;
                while (exception is not null)
                {
                    GD.PushError(exception);
                    GD.Print(exception.Message);
                    GD.Print(exception.StackTrace);
                    GD.Print("");
                    GD.Print("");
                    GD.Print("---====================----");
                    GD.Print("");
                    GD.Print("");

                    exception = exception.InnerException;
                }
            }).CallDeferred();
        }

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
