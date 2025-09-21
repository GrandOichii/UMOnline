namespace UMCore.Matches.Attacks;

using UMCore.Matches;
using UMCore.Matches.Cards;

public readonly struct AvailableAttack
{
    public required Fighter Fighter { get; init; }
    public required Fighter Target { get; init; }
    public required MatchCard AttackCard { get; init; }
}
