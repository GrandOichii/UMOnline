namespace UMDTO;

public enum MatchProcessGetStatus
{
    WAITING_FOR_PLAYERS,
    IN_PROGRESS,
    FINISHED,
    CRASHED
}

public class MatchProcessGet
{
    public required string Id { get; init; }
    public required MatchProcessGetStatus Status { get; init; }
    public required string Title { get; init; }
    public required int TeamCount { get; init; }
    public required List<string> AllowedFighters { get; init; }
    public required List<MatchProcessGetPlayer> Players { get; init; }
}

public class MatchProcessGetPlayer
{
    public required string Name { get; init; }
    public required int TeamIdx { get; init; }
    public required string? LoadoutName { get; init; }
}