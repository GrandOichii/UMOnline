using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
    public List<Log> Logs { get; }

    public MatchFrame(Match.Data data, List<Log> logs)
    {
        Data = data;
        Logs = logs;
    }

    public string GetHash()
    {
        return JsonSerializer.Serialize(Data);
    }
}

public class MatchStateRecorderPlayerControllerWrapper(IPlayerController controller) 
    : PlayerControllerWrapper(controller)
{
    public List<MatchFrame> Frames { get; } = [];
    public List<Log> NewLogs { get; } = [];
    public Match.SetupData SetupData { get; private set; }

    public override Task HandleActionChoice(string choice)
    {
        return Task.CompletedTask;
    }

    public override Task HandleSetup(Player player, Match.SetupData setupData)
    {
        SetupData = setupData;
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

    public override void HandleNewLog(Log l)
    {
        NewLogs.Add(l);
    }


    private void AddFrame(Match.Data data)
    {
        List<Log> logs = [];
        if (Frames.Count > 0)
        {
            logs = [.. Frames.Last().Logs];
        }
        logs.AddRange(NewLogs);

        var frame = new MatchFrame(data, logs);

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
    [Export]
    public Button NextStateButtonNode { get; set; }
    [Export]
    public Button PrevStateButtonNode { get; set; }
    [Export]
    public Node ReplayMatchConnectionNode { get; set; }

    #endregion

    private LocalRepository _repo;

	private MatchRecordGet _record;

    private int _frame = 0;
    private List<MatchFrame> _frames;

    public override void _Ready()
    {
        OverlayNode.Show();

        MatchDisplayNode.Call("set_connection", ReplayMatchConnectionNode);
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
                _frames = recorder.Frames;
                MatchDisplayNode.Call("load_setup", Json.ParseString(JsonSerializer.Serialize(recorder.SetupData)));
                SetFrame(0);
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

    private void SetFrame(int v)
    {
        _frame = Math.Clamp(v, 0, _frames.Count);
        
        PrevStateButtonNode.Disabled = v == 0;
        NextStateButtonNode.Disabled = v == _frames.Count - 1;

        MatchDisplayNode.Call("load_match", Json.ParseString(JsonSerializer.Serialize(_frames[_frame].Data)));

        if (OverlayNode.Visible)
            OverlayNode.Hide();
    }

    #region Signal connections

    public void OnPrevStateButtonPressed()
    {
        SetFrame(_frame - 1);
    }

    public void OnNextStateButtonPressed()
    {
        SetFrame(_frame + 1);
    }

    #endregion
}
