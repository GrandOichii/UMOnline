using UMCore.Matches.Attacks;

namespace UMCore.Matches.Players;

public struct TurnHistory(Player player)
{
    public List<string> PerformedActions { get; } = [];
    public List<Attack> Attacks { get; } = [];

    public bool PerformedAction(string action)
    {
        return PerformedActions.Contains(action);
    }

    public bool AttackedWithFighter(Fighter fighter)
    {
        return Attacks.Any((a) => a.Fighter == fighter);
    }

    public void RecordAttack(Combat combat)
    {
        Attacks.Add(new()
        {
            Fighter = combat.Attacker,
            Target = combat.Defender,
            Won = player == combat.Winner
        });
    }

    public void RecordPerformedAction(IAction action)
    {
        PerformedActions.Add(action.Name());
    }

    public int GetLostCounter() => Attacks.Count(a => !a.Won);

    public void Clear()
    {
        PerformedActions.Clear();
        Attacks.Clear();
    }

    public readonly struct Attack
    {
        public Fighter Fighter { get; init; }
        public Fighter Target { get; init; }
        public bool Won { get; init; }
    }
}