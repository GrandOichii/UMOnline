namespace UMCore.Tests.Setup.Builders;

public class MatchConfigBuilder
{
    public static MatchConfig BuildDefault() => MatchConfig.Default;

    private MatchConfig _result = new()
    {
        ActionsPerTurn = 2,
        ExhaustDamage = 1,
        FirstPlayerIdx = 0,
        InitialHandSize = 5,
        ManoeuvreDrawAmount = 1,
        MaxHandSize = 7,
        RandomFirstPlayer = false,
        RandomMatch = true,
        Seed = 0,
        TeamSize = 1
    };

    public MatchConfig Build() => _result;

    public MatchConfigBuilder TeamSize(int size)
    {
        _result.TeamSize = size;
        return this;
    }
}