using Microsoft.Extensions.Logging;

namespace UMCore.Matches.Players;

public class PlayerInitialFighterPlacer
{
    public async Task Run(Player player, int spawnNumber)
    {
        var heroQueue = new Queue<Fighter>(player.Fighters.Where(f => f.IsHero()));
        var sidekickQueue = new Queue<Fighter>(player.Fighters.Where(f => f.IsSidekick()));

        // TODO check that is hero
        var hero = heroQueue.Dequeue();
        var heroNode = player.Match.Map.GetSpawnLocation(spawnNumber);

        await heroNode.PlaceFighter(hero);
        player.Match.Logs.Public($"Player {player.FormattedLogName} placed {hero.FormattedLogName} in spawn number {spawnNumber}");


        List<MapNode> heroAvailableNodes = [.. heroNode.GetAdjacentEmptyNodes()];
        List<MapNode> sidekickAvailableNodes = [];

        await PlaceRemaining(player, heroQueue, heroAvailableNodes, sidekickAvailableNodes);
        await PlaceRemaining(player, sidekickQueue, heroAvailableNodes, sidekickAvailableNodes);
        // TODO log

        player.Match.Logs.Public($"Player {player.FormattedLogName} placed all of their fighters");
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