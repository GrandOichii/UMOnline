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

        for (int i = 1; IsActive && i < result.Nodes.Count; ++i)
        {
            var node = result.Nodes[i];
            // ! i dont like this
            if (node.Fighter is null)
            {
                await node.PlaceFighter(Fighter);
            }
        }

        await Fighter.Owner.Match.ExecuteAfterMovementEffects();
    }
    
    public void Cancel()
    {
        IsActive = false;
    }
}