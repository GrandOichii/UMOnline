using UMCore.Matches.Players;

namespace UMCore.Matches;

public class Movement(Player player, Fighter fighter, int movement, bool canMoveOverFriendly, bool canMoveOverOpposing)
{
    public Player Player { get; } = player;
    public Fighter Fighter { get; } = fighter;
    public bool IsActive { get; set; } = false;

    public async Task Resolve()
    {
        IsActive = true;

        var paths = Player.Match.Map.GetPossiblePaths(Fighter, movement, canMoveOverFriendly, canMoveOverOpposing);
        Path result = await Player.Controller.ChoosePath(Player, [.. paths], $"Choose where to move {Fighter.LogName}");
        if (result.Nodes.Count == 1)
        {
            // TODO log about not moving
            return;
        }

        var prevNode = Player.Match.Map.GetFighterLocation(Fighter);
        for (int i = 1; IsActive && i < result.Nodes.Count; ++i)
        {
            var node = result.Nodes[i];
            // ! i dont like this
            if (node.Fighter is null)
            {
                await node.PlaceFighter(Fighter);
            }

            await Player.Match.ExecuteOnMoveEffects(Fighter, prevNode, node);
            prevNode = node;
        }

        await Player.Match.ExecuteAfterMovementEffects();
    }
    
    public void Cancel()
    {
        IsActive = false;
    }
}