namespace UMCore.Matches;

public class Movement(Fighter fighter, int movement, bool canMoveOverFriendly, bool canMoveOverOpposing)
{
    public Fighter Fighter { get; } = fighter;
    public bool IsActive { get; set; } = false;

    public async Task Resolve()
    {
        IsActive = true;

        var paths = Fighter.Owner.Match.Map.GetPossiblePaths(Fighter, movement, canMoveOverFriendly, canMoveOverOpposing);
        Path result = await Fighter.Owner.Controller.ChoosePath(Fighter.Owner, [.. paths], $"Choose where to move {Fighter.LogName}");
        if (result.Nodes.Count == 1)
        {
            // TODO log about not moving
            return;
        }

        for (int i = result.Nodes.Count - 2; IsActive && i >= 0; --i)
        {
            var node = result.Nodes[i];
            await node.PlaceFighter(Fighter);
        }
        
        // TODO this doesnt work - if limiting movement to 1, the fighter will not be able to move over friendlies
        // while (IsActive && movement-- > 0)
        // {
        //     var available = Fighter.Owner.Match.Map.GetPossibleMovementResults(Fighter, 1, canMoveOverFriendly, canMoveOverOpposing);
        //     var result = await Fighter.Owner.Controller.ChooseNode(Fighter.Owner, [.. available], $"Choose where to move {Fighter.LogName} (movement left: {movement + 1})");
        //     await result.PlaceFighter(Fighter);
        // }
    }
    
    public void Cancel()
    {
        IsActive = false;
    }
}