namespace UMCore.Matches.Attacks;

using UMCore.Matches;
using UMCore.Matches.Cards;

/// <summary>
/// Available attack for a fighter
/// </summary>
public class AvailableAttack
{
    /// <summary>
    /// Attack source
    /// </summary>
    public required Fighter Fighter { get; init; }
    /// <summary>
    /// Attack target
    /// </summary>
    public required Fighter Target { get; init; }
    /// <summary>
    /// Card, used to attack
    /// </summary>
    public required MatchCard AttackCard { get; init; }
}
