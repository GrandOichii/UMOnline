using System.Threading.Tasks;
using UMCore.Matches;
using UMCore.Matches.Players;
using UMCore.Templates;

public class ConsolePlayerController : IPlayerController
{
    
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
            Zones = [0]
        };
        //0;1
        var node01 = new MapNodeTemplate()
        {
            Id = 1,
            Zones = [0]
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
            Zones = [0, 1]
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
            Zones = [0, 1]
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
            Zones = [1]
        };
        //2;2
        var node22 = new MapNodeTemplate()
        {
            Id = 22,
            Zones = [1]
        };

        return new()
        {
            Nodes = [node00, node01, node02, node10, node11, node12, node20, node21, node21],
            Adjacent = [
                .. Bidirectional(node00, node01),
                .. Bidirectional(node01, node02),
                .. Bidirectional(node02, node12),
                .. Bidirectional(node12, node22),
                .. Bidirectional(node22, node21),
                .. Bidirectional(node21, node20),
                .. Bidirectional(node20, node10),
                .. Bidirectional(node10, node00),

                .. Bidirectional(node10, node11),
            ],
            SecretPassages = [
                .. Bidirectional(node10, node12)
            ]
        };
    }

    public static async Task Main(string[] args)
    {
        var map = GetMapTemplate();

        var match = new Match(map)
        {
            Logger = null,
        };

        var controller = new ConsolePlayerController();

        await match.AddPlayer(
            "p1",
            0,
            controller
        );
        await match.AddPlayer(
            "p2",
            1,
            controller
        );

        await match.Run();
    }
}