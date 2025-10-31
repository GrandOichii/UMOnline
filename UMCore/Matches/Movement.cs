namespace UMCore.Matches;

public class Movement(Fighter fighter, int movement, bool canMoveOverFriendly, bool canMoveOverOpposing)
{
    public Fighter Fighter { get; } = fighter;
    public bool IsActive { get; set; } = false;

    public async Task Resolve()
    {
        IsActive = true;
        while (IsActive && movement-- > 0)
        {
            var available = Fighter.Owner.Match.Map.GetPossibleMovementResults(Fighter, 1, canMoveOverFriendly, canMoveOverOpposing);
            var result = await Fighter.Owner.Controller.ChooseNode(Fighter.Owner, [.. available], $"Choose where to move {Fighter.LogName} (movement left: {movement + 1})");
            await result.PlaceFighter(Fighter);
        }
    }
    
    public void Cancel()
    {
        IsActive = false;
    }
}