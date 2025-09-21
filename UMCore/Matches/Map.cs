using System.Security;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches;

public class Map
{
    public Match Match { get; }
    public MapTemplate Template { get; }
    public List<MapNode> Nodes { get; }

    public Map(Match match, MapTemplate template)
    {
        Match = match;
        Template = template;

        Nodes = [];
        Dictionary<MapNodeTemplate, MapNode> mapping = [];
        foreach (var nodeTemplate in template.Nodes)
        {
            var node = new MapNode(this, nodeTemplate);
            mapping.Add(nodeTemplate, node);
            Nodes.Add(node);
        }

        // adjacent nodes
        foreach (var pair in template.Adjacent)
        {
            mapping[pair.First].Adjacent.Add(mapping[pair.Second]);
        }

        // secret passages
        foreach (var pair in template.SecretPassages)
        {
            mapping[pair.First].SecretPassages.Add(mapping[pair.Second]);
        }
    }

    public MapNode? GetFighterLocation(Fighter fighter)
    {
        return Nodes.FirstOrDefault(node => node.Fighter == fighter);
    }

    public MapNode GetSpawnLocation(int idx)
    {
        return Nodes.First(node => node.Template.SpawnNumber == idx);
    }

    public IEnumerable<MapNode> GetPossibleMovementResults(
        Fighter fighter,
        int movement,
        bool canMoveOverFriendly,
        bool canMoveOverOpposing
    ){
        // ! replace with GetRoutesTo()
        // then filter all routes using canMoveOverFriendly and canMoveOverOpposing
        
        var node = GetFighterLocation(fighter)
            ?? throw new Exception($"Failed to find fighter {fighter.LogName} for {nameof(GetPossibleMovementResults)}"); // TODO type

        HashSet<MapNode> result = [];
        HashSet<MapNode> processed = [];
        node.GetPossibleMovementResults(
            fighter.Owner,
            movement,
            canMoveOverFriendly,
            canMoveOverOpposing,
            in result,
            in processed
        );

        return result;
    }
}

public class MapNode
{
    public MapNodeTemplate Template { get; }
    public int Id { get; }
    public List<MapNode> Adjacent { get; }
    public List<MapNode> SecretPassages { get; }

    public Fighter? Fighter { get; private set; }
    // public List<Token> Tokens { get; } // TODO
    public Map Parent { get; }

    public MapNode(Map parent, MapNodeTemplate template)
    {
        Parent = parent;
        Template = template;
        Id = template.Id;
        Fighter = null;

        Adjacent = [];
        SecretPassages = [];
    }


    public async Task PlaceFighter(Fighter fighter)
    {
        var existing = Parent.GetFighterLocation(fighter);
        if (existing is not null)
        {
            // TODO
            await existing.RemoveFighter();
        }

        Fighter = fighter;
        Parent.Match.Logger?.LogDebug("Placed fighter {LogName} in node {NodeId}", fighter.LogName, Id);
        // TODO update players
    }

    public async Task RemoveFighter(bool updateClients = false)
    {
        Fighter = null;
        if (!updateClients) return;
        // TODO update clients
    }

    public void GetPossibleMovementResults(
        Player player,
        int movement,
        bool canMoveOverFriendly,
        bool canMoveOverOpposing,
        in HashSet<MapNode> result,
        in HashSet<MapNode> processed
    )
    {
        // TODO this requires testing
        if (processed.Contains(this))
        {
            return;
        }

        processed.Add(this);

        if (Fighter is null)
        {
            result.Add(this);
        } else
        {
            if (
                (!canMoveOverOpposing && Fighter.IsOpposingTo(player)) ||
                (!canMoveOverFriendly && Fighter.IsFriendlyTo(player))
            )
            {
            } else
            {
                result.Add(this);
            }
        }
        if (
            Fighter is null ||
            (canMoveOverOpposing && Fighter.IsOpposingTo(player)) ||
            (canMoveOverFriendly && Fighter.IsFriendlyTo(player))
        ){
            result.Add(this);
        }

        if (movement == 0)
        {
            return;
        }

        foreach (var node in Adjacent)
        {
            node.GetPossibleMovementResults(player, movement - 1, canMoveOverFriendly, canMoveOverOpposing, result, processed);
        }
        foreach (var node in SecretPassages)
        {
            node.GetPossibleMovementResults(player, movement - 1, canMoveOverFriendly, canMoveOverOpposing, result, processed);
        }
        // TODO add support for fog token-like effects
    }

}