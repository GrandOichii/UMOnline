namespace UMCore.Matches.Players;

public struct TurnHistory(Player player)
{
    public List<string> PerformedActions { get; } = [];
    public List<(Fighter, Fighter)> Attacks { get; } = [];

    public void Attacked(Fighter fighter, Fighter target)
    {
        Attacks.Add((fighter, target));
    }

    public void PerformedAction(IAction action)
    {
        PerformedActions.Add(action.Name());
    }

    public void Clear()
    {
        PerformedActions.Clear();
        Attacks.Clear();
    }
}