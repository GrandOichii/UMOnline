using Microsoft.Extensions.Logging;

namespace UMCore.Matches.Players;

public interface IPlayerInitialFighterPlacer
{
    Task Run(Player player, int spawnNumber, Queue<Fighter> heroQueue, Queue<Fighter> sidekickQueue); 
}

public class PlayerInitialFighterPlacerInZone : IPlayerInitialFighterPlacer
{
    // if no space left, place in any node

    public async Task Run(Player player, int spawnNumber, Queue<Fighter> heroQueue, Queue<Fighter> sidekickQueue)
    {
        // place fighter in initial node 

        var hero = heroQueue.Dequeue();
        var heroNode = player.Match.Map.GetSpawnLocation(spawnNumber);

        await heroNode.PlaceFighter(hero);
        player.Match.Logs.Public($"Player {player.FormattedLogName} placed {hero.FormattedLogName} in spawn number {spawnNumber}");

        // place all remaining fighters in nodes in the zones that the initial hero occupies
        var options = new List<MapNode>();
        foreach (var zone in heroNode.GetZones())
        {
            options.AddRange(player.Match.Map.GetNodesInZone(zone).Where(n => n.IsEmpty() && n.Template.SpawnNumber <= 0));
        }

        await PlaceRemaining(player, heroQueue, options);
        await PlaceRemaining(player, sidekickQueue, options);

        player.Match.Logs.Public($"Player {player.FormattedLogName} placed all of their fighters");

        foreach (var fighter in player.Fighters)
        {
            fighter.ExecuteWhenPlacedEffects();
        }
    }

    private async Task PlaceRemaining(Player player, Queue<Fighter> fighters, List<MapNode> options)
    {
        while (fighters.Count > 0)
        {
            var fighter = fighters.Dequeue();
            if (options.Count == 0)
            {
                options.AddRange(player.Match.Map.GetEmptyNodes());
            }
            var node = await player.Controller.ChooseNode(player, [.. options], $"Choose where to place {fighter.FormattedLogName}");
            options.Remove(node);
            await node.PlaceFighter(fighter);
            player.Match.Logs.Public($"Player {player.FormattedLogName} placed {fighter.FormattedLogName}");
        }
        
    }
}

public class PlayerInitialFighterPlacerNeighbors : IPlayerInitialFighterPlacer
{
    public async Task Run(Player player, int spawnNumber, Queue<Fighter> heroQueue, Queue<Fighter> sidekickQueue)
    {
        // TODO check that is hero
        var hero = heroQueue.Dequeue();
        var heroNode = player.Match.Map.GetSpawnLocation(spawnNumber);

        await heroNode.PlaceFighter(hero);
        player.Match.Logs.Public($"Player {player.FormattedLogName} placed {hero.FormattedLogName} in spawn number {spawnNumber}");


        List<MapNode> heroAvailableNodes = [.. heroNode.GetAdjacentEmptyNodes()];
        List<MapNode> sidekickAvailableNodes = [];

        await PlaceRemaining(player, heroQueue, heroAvailableNodes, sidekickAvailableNodes);
        await PlaceRemaining(player, sidekickQueue, heroAvailableNodes, sidekickAvailableNodes);

        player.Match.Logs.Public($"Player {player.FormattedLogName} placed all of their fighters");

        foreach (var fighter in player.Fighters)
        {
            fighter.ExecuteWhenPlacedEffects();
        }
    }

    private async Task PlaceRemaining(Player player, Queue<Fighter> fighters, List<MapNode> heroAvailableNodes, List<MapNode> sidekickAvailableNodes)
    {
        while (fighters.Count > 0)
        {
            var fighter = fighters.Dequeue();
            List<MapNode> availableNodes = [.. heroAvailableNodes];
            if (availableNodes.Count == 0)
            {
                availableNodes.AddRange(sidekickAvailableNodes);
            }
            var node = availableNodes[0];

            if (fighters.Count + 1 < availableNodes.Count)
            {
                node = await player.Controller.ChooseNode(player, [.. availableNodes], $"Choose where to place {fighter.FormattedLogName}");
            }
            await node.PlaceFighter(fighter);
            player.Match.Logs.Public($"Player {player.FormattedLogName} placed {fighter.FormattedLogName}");

            ModAvailable(heroAvailableNodes, sidekickAvailableNodes, node);

            (fighter.IsHero()
                ? heroAvailableNodes
                : sidekickAvailableNodes).AddRange(node.GetAdjacentEmptyNodes());
        }
    }

    private static void ModAvailable(List<MapNode> heroAvailableNodes, List<MapNode> sidekickAvailableNodes, MapNode node)
    {
        if (heroAvailableNodes.Remove(node))
        {
            return;
        }
        if (sidekickAvailableNodes.Remove(node))
        {
            return;
        }
        // TODO throw
    }
}