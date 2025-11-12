using UMCore.Matches.Effects;
using UMCore.Matches.Players;

namespace UMCore.Matches;

public class Movement(Player player, Fighter fighter, int movement, bool canMoveOverFriendly, bool canMoveOverOpposing)
{
    public HashSet<Fighter> MovedThroughFighters { get; } = [];
    public Player Player { get; } = player;
    public Fighter Fighter { get; } = fighter;
    public bool IsActive { get; set; } = false;
    public bool CanMoveOverFriendly { get; set; } = canMoveOverFriendly;
    public bool CanMoveOverOpposing { get; set; } = canMoveOverOpposing;
    public int Value { get; set; } = movement;
    public List<(Fighter, EffectCollection)> AtTheEndOfMovementEffects { get; } = [];

    public async Task Resolve()
    {
        IsActive = true;

        var paths = Player.Match.Map.GetPossiblePaths(Fighter, Value, CanMoveOverFriendly, CanMoveOverOpposing);
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
            else
            {
                if (node.Fighter != Fighter)
                    MovedThroughFighters.Add(node.Fighter);
            }

            await Player.Match.ExecuteOnMoveEffects(Fighter, prevNode, node);
            prevNode = node;
        }

        // TODO order effects
        foreach (var (source, effect) in AtTheEndOfMovementEffects)
            effect.Execute(new(source), new()); // TODO? subjects
        await Player.Match.ExecuteAfterMovementEffects();
    }

    public void Cancel()
    {
        IsActive = false;
    }
}