using UMCore.Matches;
using UMCore.Matches.Players.Controllers;

namespace UMDTO;

public class PlayerRecordGet
{
    public required string Name { get; init; }
    public required int TeamIdx { get; init; }
    public required string Loadout { get; init; }
    public required PlayerControllerRecord Responses { get; init; }
}

public class MatchRecordGet
{
    public required string Id { get; init; }
    public required MatchConfig Config { get; init; }
    public required int Seed { get; init; }
    public required List<PlayerRecordGet> Players { get; init; }
}