using System.Security;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches;

public class Map : IHasData<Map.Data>
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

    public IEnumerable<Fighter> GetReachableFighters(Fighter fighter, int range)
    {
        var node = GetFighterLocation(fighter)
            ?? throw new Exception($"Failed to find fighter {fighter.LogName} for {nameof(GetReachableFighters)}"); // TODO type

        HashSet<Fighter> result = [];
        node.GetReachableFighters(
            fighter,
            range,
            in result
        );

        return result;
    }

    public IEnumerable<Fighter> GetRangedReachableFighters(Fighter fighter)
    {
        var node = GetFighterLocation(fighter)
            ?? throw new Exception($"Failed to find fighter {fighter.LogName} for {nameof(GetReachableFighters)}"); // TODO type

        HashSet<Fighter> result = [];

        var zones = node.GetZones();
        foreach (var zone in zones)
            foreach (var n in GetNodesInZone(zone))
                if (n.Fighter != null && n.Fighter.IsOpposingTo(fighter.Owner))
                    result.Add(n.Fighter);

        return result;
    }

    public IEnumerable<MapNode> GetNodesInZone(int zone) => Nodes.Where(n => n.IsInZone([zone]));

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
    )
    {
        var node = GetFighterLocation(fighter)
            ?? throw new Exception($"Failed to find fighter {fighter.LogName} for {nameof(GetPossibleMovementResults)}"); // TODO type

        HashSet<MapNode> result = [];
        node.GetPossibleMovementResults(
            fighter,
            movement,
            canMoveOverFriendly,
            canMoveOverOpposing,
            in result
        );

        return result;
    }

    public async Task RemoveFighterFromBoard(Fighter fighter)
    {
        var node = GetFighterLocation(fighter);
        // TODO check for null
        await node!.RemoveFighter(true);
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Nodes = [.. Nodes.Select(n => n.GetData(player))]
        };
    }

    public class Data
    {
        public required MapNode.Data[] Nodes { get; init; }
    }
}

public class MapNode : IHasData<MapNode.Data>
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

    public IEnumerable<int> GetZones() => Template.Zones;

    public void GetReachableFighters(
        Fighter fighter,
        int range,
        in HashSet<Fighter> result)
    {
        if (
            Fighter is not null &&
            Fighter != fighter &&
            Fighter.IsOpposingTo(fighter.Owner)
            )
        {
            result.Add(Fighter);
        }

        if (range == 0) return;

        foreach (var node in Adjacent)
        {
            node.GetReachableFighters(fighter, range - 1, in result);
        }
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
        await Parent.Match.UpdateClients();
    }

    public async Task RemoveFighter(bool updateClients = false)
    {
        Fighter = null;
        if (!updateClients) return;

        await Parent.Match.UpdateClients();
    }

    public void GetPossibleMovementResults(
        Fighter fighter,
        int movement,
        bool canMoveOverFriendly,
        bool canMoveOverOpposing,
        in HashSet<MapNode> result)
    {
        // TODO this requires testing

        if (Fighter is not null && Fighter != fighter)
        {
            if (!canMoveOverOpposing && Fighter.IsOpposingTo(fighter.Owner)) return;
            if (!canMoveOverFriendly && Fighter.IsFriendlyTo(fighter.Owner)) return;
        }
        else
        {
            result.Add(this);
        }

        if (movement == 0)
        {
            return;
        }

        foreach (var node in Adjacent)
        {
            node.GetPossibleMovementResults(fighter, movement - 1, canMoveOverFriendly, canMoveOverOpposing, result);
        }
        foreach (var node in SecretPassages)
        {
            node.GetPossibleMovementResults(fighter, movement - 1, canMoveOverFriendly, canMoveOverOpposing, result);
        }
        // TODO add support for fog token-like effects
    }

    public bool IsInZone(IEnumerable<int> zones)
    {
        return GetZones().Intersect(zones).Any();
    }

    public bool IsAdjecentTo(MapNode other)
    {
        return other.Adjacent.Contains(this);
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Id = Id,
            // Fighter = Fighter?.GetData(player),
            FighterId = Fighter?.Id,
        };
    }

    public class Data
    {
        public required int Id { get; init; }       
        // public required Fighter.Data? Fighter { get; init; }
        public required int? FighterId { get; init; }
    }
}