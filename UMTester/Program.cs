using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UMCore.Matches;
using UMCore.Matches.Attacks;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Templates;

public class ConsolePlayerController : IPlayerController
{
    private void PrintInfo(Fighter fighter)
    {
        System.Console.WriteLine($"\t{fighter.LogName}: {fighter.Health.Current}/{fighter.Health.Max}");
    }

    private void PrintInfo(Player player)
    {

        foreach (var node in player.Match.Map.Nodes)
            System.Console.WriteLine($"{node.Id} -> {node.Fighter?.LogName}");
        foreach (var fighter in player.Fighters)
            PrintInfo(fighter);

        System.Console.WriteLine($"-= {player.LogName} =-");
        System.Console.WriteLine($"Hand count: {player.Hand.Count}");
        System.Console.WriteLine($"Deck count: {player.Deck.Count}");
        System.Console.WriteLine($"Discard count: {player.DiscardPile.Count}");
        System.Console.WriteLine($"Actions left: {player.ActionCount}");
    }
    
    public async Task<string> ChooseAction(Player player, string[] options)
    {
        PrintInfo(player);
        System.Console.WriteLine($"Choose action: {string.Join(", ", options)}");
        var nodes = options.ToList();
        for (int i = 0; i < nodes.Count; ++i)
            System.Console.WriteLine($"{i}: {nodes[i]}");
        var result = Console.ReadLine()!;
        return nodes[int.Parse(result)];
    }

    public async Task<MapNode> ChooseNode(Player player, IEnumerable<MapNode> options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var nodes = options.ToList();
        for (int i = 0; i < nodes.Count; ++i)
            System.Console.WriteLine($"{i}: {nodes[i].Id}");
        var result = Console.ReadLine()!;
        return nodes[int.Parse(result)];
    }

    public async Task<MatchCard> ChooseCardInHand(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var cards = options.ToList();
        for (int i = 0; i < cards.Count; ++i)
            System.Console.WriteLine($"{i}: {cards[i].LogName}");
        var result = Console.ReadLine()!;
        return cards[int.Parse(result)];
    }
    
    public async Task<MatchCard?> ChooseCardInHandOrNothing(Player player, int playerHandIdx, IEnumerable<MatchCard> options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var cards = options.ToList();
        for (int i = 0; i < cards.Count; ++i)
            System.Console.WriteLine($"{i}: {cards[i].LogName}");
        var result = Console.ReadLine()!;
        if (result == "") return null;
        return cards[int.Parse(result)];
    }

    public async Task<Fighter> ChooseFighter(Player player, IEnumerable<Fighter> options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var fighters = options.ToList();
        for (int i = 0; i < fighters.Count; ++i)
            System.Console.WriteLine($"{i}: {fighters[i].LogName}");
        var result = Console.ReadLine()!;
        return fighters[int.Parse(result)];
    }

    public async Task<AvailableAttack> ChooseAttack(Player player, IEnumerable<AvailableAttack> options)
    {
        PrintInfo(player);
        System.Console.WriteLine("Choose how to attack");
        var attacks = options.ToList();
        for (int i = 0; i < attacks.Count; ++i)
            System.Console.WriteLine($"{i}: {attacks[i].Fighter.LogName} -> {attacks[i].Target.LogName} [{attacks[i].AttackCard.LogName}]");
        var result = Console.ReadLine()!;
        return attacks[int.Parse(result)];   
    }

    public async Task<string> ChooseString(Player player, IEnumerable<string> options, string hint)
    {
        PrintInfo(player);
        System.Console.WriteLine(hint);
        var opts = options.ToList();
        for (int i = 0; i < opts.Count; ++i)
            System.Console.WriteLine($"{i}: {opts[i]}");
        var result = Console.ReadLine()!;
        return opts[int.Parse(result)];
    }

    public Task Update(Player player)
    {
        throw new NotImplementedException();
    }
}

public class Program
{
    public static IEnumerable<MapNodeLinkTemplate> Bidirectional(MapNodeTemplate n1, MapNodeTemplate n2)
    {
        return [
            new() {
                First = n1,
                Second = n2,
            },
            new() {
                First = n2,
                Second = n1,
            }
        ];
    }

    public static MapTemplate GetMapTemplate()
    {
        // o-0-o
        // | | |
        // o o o
        // |   |
        // o-0-o

        //0;0
        var node00 = new MapNodeTemplate()
        {
            Id = 0,
            Zones = [0],
            SpawnNumber = 0,
        };
        //0;1
        var node01 = new MapNodeTemplate()
        {
            Id = 1,
            Zones = [0],
            SpawnNumber = 2
        };
        //0;2
        var node02 = new MapNodeTemplate()
        {
            Id = 2,
            Zones = [0]
        };
        //1;0
        var node10 = new MapNodeTemplate()
        {
            Id = 10,
            Zones = [0, 1],
            SpawnNumber = 1,
        };
        //1;1
        var node11 = new MapNodeTemplate()
        {
            Id = 11,
            Zones = [0, 1]
        };
        //1;2
        var node12 = new MapNodeTemplate()
        {
            Id = 12,
            Zones = [0, 1],
            SpawnNumber = 3
        };
        //2;0
        var node20 = new MapNodeTemplate()
        {
            Id = 20,
            Zones = [1]
        };
        //2;1
        var node21 = new MapNodeTemplate()
        {
            Id = 21,
            Zones = [1],
        };
        //2;2
        var node22 = new MapNodeTemplate()
        {
            Id = 22,
            Zones = [1],
        };

        return new()
        {
            Nodes = [node00, node01, node02, node10, node11, node12, node20, node21, node22],
            Adjacent = [
                .. Bidirectional(node00, node01),
                .. Bidirectional(node01, node02),
                .. Bidirectional(node02, node12),
                .. Bidirectional(node12, node22),
                .. Bidirectional(node22, node21),
                .. Bidirectional(node21, node20),
                .. Bidirectional(node20, node10),
                .. Bidirectional(node10, node00),

                .. Bidirectional(node01, node11),
            ],
            SecretPassages = [
                .. Bidirectional(node01, node21)
            ]
        };
    }

    private static LoadoutTemplate LoadLoadout(string path)
    {
        var data = File.ReadAllText(path);
        var result = JsonSerializer.Deserialize<LoadoutTemplate>(data)!;
        foreach (var card in result.Deck)
        {
            card.Card.Script = File.ReadAllText(card.Card.Script);
        }

        foreach (var fighter in result.Fighters)
        {
            fighter.Script = File.ReadAllText(fighter.Script);
        }
        return result;
    }

    public static async Task Main(string[] args)
    {
        var map = GetMapTemplate();

        var match = new Match(map, File.ReadAllText("../core.lua"))
        {
            Logger = LoggerFactory
                .Create(builder => builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug)
                )
                .CreateLogger("UMTester")
        };

        var controller = new ConsolePlayerController();

        var loadout = LoadLoadout("../loadouts/foobar.json");

        await match.AddPlayer(
            "p1",
            0,
            loadout,
            controller
        );
        await match.AddPlayer(
            "p2",
            1,
            loadout,
            controller
        );

        try
        {
            await match.Run();
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            System.Console.WriteLine(e.StackTrace);
        }
    }

}
