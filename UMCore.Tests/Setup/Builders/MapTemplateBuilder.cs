namespace UMCore.Tests.Setup.Builders;

public class MapTemplateBuilder
{
    public static MapNodeLinkTemplate[] Bidirectional(MapNodeTemplate n1, MapNodeTemplate n2)
    {
        return [
            new() {
                First = n1.Id,
                Second = n2.Id,
            },
        ];
    }

    public MapTemplate Result { get; } = new()
    {
        Nodes = [],
        Adjacent = [],
    };

    public static MapTemplate Build2x2()
    {
        return new MapTemplateBuilder()
            .AddNode(00, [0], spawnNumber: 1)
            .AddNode(01, [0], spawnNumber: 2)
            .AddNode(10, [0], spawnNumber: 3)
            .AddNode(11, [0], spawnNumber: 4)
            .Connect(00, 01)
            .Connect(00, 11)
            .Connect(00, 10)
            .Connect(01, 11)
            .Connect(01, 10)
            .Connect(11, 10)
            .Build();
    }

    private readonly Dictionary<int, MapNodeTemplate> _nodeMap = [];

    public MapTemplateBuilder AddNode(
        int id,
        int[] zones,
        bool HasSecretPassage = false,
        int spawnNumber = -1
    ) {
        if (NodeExists(id))
        {
            throw new Exception($"Tried to add map node with duplicate id: {id}");
        }

        var node = new MapNodeTemplate()
        {
            Id = id,
            Zones = [.. zones],
            HasSecretPassage = HasSecretPassage,
            SpawnNumber = spawnNumber
        };

        _nodeMap.Add(id, node);
        Result.Nodes.Add(node);

        return this;
    }

    public MapTemplateBuilder Connect(int id1, int id2)
    {
        if (!NodeExists(id1))
        {
            throw new Exception($"No node with id {id1}");
        }
        if (!NodeExists(id2))
        {
            throw new Exception($"No node with id {id2}");
        }
        Result.Adjacent.Add(new()
        {
            First = id1,
            Second = id2,
        });
        return this;
    }

    public MapTemplateBuilder ConnectAll()
    {
        for (int i = 0; i < Result.Nodes.Count; ++i)
        {
            for (int ii = i + 1; ii < Result.Nodes.Count; ++ii)
            {
                Connect(Result.Nodes[i].Id, Result.Nodes[ii].Id);
            }
        }

        return this;
    }
    
    private bool NodeExists(int id)
    {
        return _nodeMap.ContainsKey(id);
    } 

    public MapTemplate Build()
    {
        return Result;
    }
}