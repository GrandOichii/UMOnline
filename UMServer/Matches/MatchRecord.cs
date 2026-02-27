namespace UMServer.Matches;

public class MatchRecord(int seed, MatchProcess match)
{
    public int Seed { get; init; } = seed;
}