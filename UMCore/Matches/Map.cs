using System.Diagnostics;
using System.Security;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Templates;

namespace UMCore.Matches;

public class Map : IHasData<Map.Data>
{
    public Match Match { get; }
    public MapTemplate Template { get; }
    public List<MapNode> Nodes { get; }
    public List<MapNode> SecretPassageNodes { get; }

    public Map(Match match, MapTemplate template)
    {
        Match = match;
        Template = template;

        SecretPassageNodes = [];
        Nodes = [];

        Dictionary<MapNodeTemplate, MapNode> mapping = [];
        foreach (var nodeTemplate in template.Nodes)
        {
            var node = new MapNode(this, nodeTemplate);
            mapping.Add(nodeTemplate, node);
            Nodes.Add(node);
            if (nodeTemplate.HasSecretPassage)
                SecretPassageNodes.Add(node);
        }

        // adjacent nodes
        foreach (var pair in template.Adjacent)
        {
            var n1 = Template.GetNode(pair.First);
            var n2 = Template.GetNode(pair.Second);
            mapping[n1].Adjacent.Add(mapping[n2]);
            if (pair.Bidirectional)
            {
                mapping[n2].Adjacent.Add(mapping[n1]);
            }
        }
    }

    public List<Path> GetPossiblePaths(
        Fighter fighter,
        int movement,
        bool canMoveOverFriendly,
        bool canMoveOverOpposing
    )
    {
        var result = GetFighterLocation(fighter).GetPossiblePaths(fighter, movement, canMoveOverFriendly, canMoveOverOpposing, []);
        return result;
    }

    public IEnumerable<Fighter> GetReachableFighters(Fighter fighter, int range)
    {
        var node = GetFighterLocation(fighter);

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
        var node = GetFighterLocation(fighter);

        HashSet<Fighter> result = [];

        var zones = node.GetZones();
        foreach (var zone in zones)
            foreach (var n in GetNodesInZone(zone))
                if (n.Fighter != null && n.Fighter.IsOpposingTo(fighter.Owner))
                    result.Add(n.Fighter);

        return result;
    }

    public IEnumerable<MapNode> GetNodesInZone(int zone) => Nodes.Where(n => n.IsInZone([zone]));

    public MapNode GetFighterLocation(Fighter fighter)
    {
        var result = GetFighterLocationOrDefault(fighter);
        if (result is null)
        {
            throw new Exception($"Failed to find fighter {fighter.LogName}");
        }
        return result;
    }

    public MapNode? GetFighterLocationOrDefault(Fighter fighter)
    {
        return Nodes.FirstOrDefault(node => node.Fighter == fighter);
    }

    public IEnumerable<MapNode> GetEmptyNodes() => Nodes.Where(n => n.IsEmpty());

    public MapNode GetSpawnLocation(int idx)
    {
        return Nodes.First(node => node.Template.SpawnNumber == idx);
    }

    // public IEnumerable<MapNode> GetPossibleMovementResults(
    //     Fighter fighter,
    //     int movement,
    //     bool canMoveOverFriendly,
    //     bool canMoveOverOpposing
    // )
    // {
    //     var node = GetFighterLocation(fighter);

    //     HashSet<MapNode> result = [];
    //     node.GetPossibleMovementResults(
    //         fighter,
    //         movement,
    //         canMoveOverFriendly,
    //         canMoveOverOpposing,
    //         in result
    //     );

    //     return result;
    // }

    public async Task RemoveFighterFromBoard(Fighter fighter)
    {
        var node = GetFighterLocation(fighter);
        await Match.ExecuteOnMoveEffects(fighter, node, null);
        
        await node.RemoveFighter(true);
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

    public Fighter? Fighter { get; private set; }
    public List<PlacedToken> Tokens { get; }
    public Map Parent { get; }

    public MapNode(Map parent, MapNodeTemplate template)
    {
        Parent = parent;
        Template = template;
        Id = template.Id;
        Fighter = null;

        Adjacent = [];
        Tokens = [];
    }

    public IEnumerable<MapNode> GetAdjacentEmptyNodes() => [.. Adjacent.Where(n => n.IsEmpty())];

    public bool IsEmpty() => Fighter == null;

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
        var existing = Parent.GetFighterLocationOrDefault(fighter);
        if (existing is not null)
        {
            await existing.RemoveFighter();
        }

        Fighter = fighter;
        Parent.Match.Logger?.LogDebug("Placed fighter {LogName} in node {NodeId}", fighter.LogName, Id);
        await Parent.Match.UpdateClients();

        await ResolveOnStepEffects();
        // TODO trigger OnStep effects of all tokens
    }

    private async Task ResolveOnStepEffects()
    {
        List<(PlacedToken, EffectCollection)> effects = [];
        foreach (var token in Tokens)
        {
            effects.AddRange(token.GetOnStepEffects(Fighter!).Select(s => (token, s)));
        }

        // TODO order effects
        foreach (var pair in effects)
        {
            var token = pair.Item1;
            var effect = pair.Item2;
            effect.Execute(new(token.Original.Originator, token), new(Fighter!));
        }

        await Parent.Match.UpdateClients();
    }

    public async Task RemoveFighter(bool updateClients = false)
    {
        Fighter = null;
        if (!updateClients) return;

        await Parent.Match.UpdateClients();
    }

    private bool CanStepOver(Fighter fighter, bool canMoveOverFriendly, bool canMoveOverOpposing)
    {
        if (Fighter is null || Fighter == fighter)
        {
            return true;
        }
        if (!canMoveOverOpposing && Fighter.IsOpposingTo(fighter.Owner))
            return false;
        if (!canMoveOverFriendly && Fighter.IsFriendlyTo(fighter.Owner))
            return false;

        return true;
    }

    public List<Path> GetPossiblePaths(
        Fighter fighter,
        int movement,
        bool canMoveOverFriendly,
        bool canMoveOverOpposing,
        HashSet<MapNode> processed
    )
    {
        // if (processed.Contains(this)) return [];
        // processed.Add(this);

        var canStepOver = CanStepOver(fighter, canMoveOverFriendly, canMoveOverOpposing);
        var isOccupied = Fighter is not null && Fighter != fighter;
        if (!canStepOver)
        {
            return [];
        }

        if (movement == 0 && isOccupied)
        {
            return [];
        }

        if (movement == 0)
        {
            return [new() {
                Nodes = [this]
            }];
        }

        List<Path> result = [];
        if (!isOccupied)
        {
            result.Add(new()
            {
                Nodes = []
            });
        }
        List<MapNode> neighbors = [.. Adjacent];
        if (Template.HasSecretPassage) 
            neighbors.AddRange(Parent.SecretPassageNodes.Where(n => n != this));
        neighbors.AddRange(Parent.Match.GetAdditionalConnectedNodesFor(fighter, this));
        
        foreach (var node in neighbors)
        {
            var paths = node.GetPossiblePaths(fighter, movement - 1, canMoveOverFriendly, canMoveOverOpposing, processed);
            result.AddRange(paths);
        }

        foreach (var path in result)
            path.Nodes.Insert(0, this);

        return result;
    }

    // public void GetPossibleMovementResults(
    //     Fighter fighter,
    //     int movement,
    //     bool canMoveOverFriendly,
    //     bool canMoveOverOpposing,
    //     in HashSet<MapNode> result)
    // {
    //     if (Fighter is not null && Fighter != fighter)
    //     {
    //         if (!canMoveOverOpposing && Fighter.IsOpposingTo(fighter.Owner)) return;
    //         if (!canMoveOverFriendly && Fighter.IsFriendlyTo(fighter.Owner)) return;
    //     }
    //     else
    //     {
    //         result.Add(this);
    //     }

    //     if (movement == 0)
    //     {
    //         return;
    //     }

    //     foreach (var node in Adjacent)
    //     {
    //         node.GetPossibleMovementResults(fighter, movement - 1, canMoveOverFriendly, canMoveOverOpposing, result);
    //     }
    //     if (Template.HasSecretPassage)
    //     {
    //         foreach (var node in Parent.SecretPassageNodes)
    //         {
    //             if (node == this) continue;
    //             node.GetPossibleMovementResults(fighter, movement - 1, canMoveOverFriendly, canMoveOverOpposing, result);
    //         }
    //     }
    //     // TODO add support for fog token-like effects
    // }

    public bool IsInZone(IEnumerable<int> zones)
    {
        return GetZones().Intersect(zones).Any();
    }

    public bool IsAdjecentTo(MapNode other)
    {
        return other.Adjacent.Contains(this);
    }

    public bool IsOccupied()
    {
        return Fighter is not null || Tokens.Count > 0;
    }

    public PlacedToken? GetTokenOrDefault(string tokenName)
    {
        return Tokens.FirstOrDefault(t => t.GetName() == tokenName);
    }

    public bool HasToken(string tokenName)
    {
        return GetTokenOrDefault(tokenName) is not null;
    }

    public async Task PlaceToken(Token token)
    {
        var newToken = token.CreatePlacedToken(this);
        if (newToken is null)
        {
            Parent.Match.Logs.Public($"No more {token.Name} tokens left!");
            return;
        }
        Tokens.Add(newToken);

        Parent.Match.Logger?.LogDebug("Placed {tokenName} token on node {nodeId} (amount left: {tokenAmount})", token.Name, Id, token.Amount);
        await Parent.Match.UpdateClients();
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Id = Id,
            Fighter = Fighter?.GetData(player),
            Tokens = [.. Tokens.Select(t => t.Original.Name)]
        };
    }

    public class Data
    {
        public required int Id { get; init; }
        public required Fighter.Data? Fighter { get; init; }
        public required string[] Tokens { get; init; }
    }
}