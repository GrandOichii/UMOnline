namespace UMCore.Matches;

public class MatchConfig
{
    public required bool RandomMatch { get; set; }
    public required int Seed { get; set; }
    public required int InitialHandSize { get; set; }
    public required int ActionsPerTurn { get; set; }
    public required int MaxHandSize { get; set; }
    public required int ManoeuvreDrawAmount { get; set; }
    public required bool RandomFirstPlayer { get; set; }
    public required int FirstPlayerIdx { get; set; }
    public required int ExhaustDamage { get; set; }
    public required int TeamSize { get; set; }

    public static readonly MatchConfig Default = new()
    {
        RandomMatch = false, // true
        Seed = 1,
        ActionsPerTurn = 2,
        InitialHandSize = 5,
        ManoeuvreDrawAmount = 1,
        MaxHandSize = 7,
        FirstPlayerIdx = 0, // -1
        RandomFirstPlayer = false, // true
        ExhaustDamage = 2,
        TeamSize = 1,
    };
}