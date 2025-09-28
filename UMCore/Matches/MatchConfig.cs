namespace UMCore.Matches;

public class MatchConfig
{
    public required bool RandomMatch { get; init; }
    public required int Seed { get; init; }
    public required int InitialHandSize { get; init; }
    public required int ActionsPerTurn { get; init; }
    public required int MaxHandSize { get; init; }
    public required int ManoeuvreDrawAmount { get; init; }
    public required bool RandomFirstPlayer { get; init; }
    public required int FirstPlayerIdx { get; init; }
    public required int ExhaustDamage { get; init; }

    public static readonly MatchConfig Default = new()
    {
        RandomMatch = true,
        Seed = 0,
        ActionsPerTurn = 2,
        InitialHandSize = 5,
        ManoeuvreDrawAmount = 1,
        MaxHandSize = 7,
        FirstPlayerIdx = -1, // -1
        RandomFirstPlayer = true, // true
        ExhaustDamage = 2,
    };
}