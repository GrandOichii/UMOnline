namespace UMCore.Matches;

public class MatchConfig
{
    public required int InitialHandSize { get; init; }
    public required int ActionsPerTurn { get; init; }
    public required int MaxHandSize { get; init; }
    public required int ManoeuvreDrawAmount { get; init; }

    public static readonly MatchConfig Default = new()
    {
        ActionsPerTurn = 2,
        InitialHandSize = 5,
        ManoeuvreDrawAmount = 1,
        MaxHandSize = 7
    };
}