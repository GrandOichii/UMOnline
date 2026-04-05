using UMCore.Matches;
using UMCore.Matches.Players.Controllers;
using UMDTO;

namespace UMServer.Matches;

public class PlayerRecord
{
    public required string Name { get; init; }
    public required int TeamIdx { get; init; }
    public required string Loadout { get; init; }
    public required RecorderControllerWrapper Recorder { get; init; }
}

public class MatchRecord(string id, int seed, MatchConfig config)
{
    public int Seed { get; init; } = seed;
    public MatchConfig Config { get; } = config;

    public List<PlayerRecord> Recorders { get; } = [];

    public void AddRecorderPlayerController(ConnectedPlayer player, RecorderControllerWrapper recorder)
    {
        Recorders.Add(new()
        {
            TeamIdx = player.TeamIdx,
            Loadout = player.Loadout!.Name,
            Name = player.Client.Name,
            Recorder = recorder
        });
    }

    public MatchRecordGet ToMatchRecordGet()
    {
        return new()
        {
            Id = id,
            Config = Config,
            Seed = Seed,
            Players = [.. Recorders.Select(r => new PlayerRecordGet()
            {
                Name = r.Name,
                Loadout = r.Loadout,
                TeamIdx = r.TeamIdx,
                Responses = r.Recorder.Record
            })]
        };
    }
}