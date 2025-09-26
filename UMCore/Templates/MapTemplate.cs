namespace UMCore.Templates;

public class MapNodeTemplate
{
    public required int Id { get; init; }
    public required List<int> Zones { get; init; }
    public int SpawnNumber { get; init; } = 0;
    public bool HasSecretPassage { get; init; } = false;
}

public class MapNodeLinkTemplate
{
    public required MapNodeTemplate First { get; init; }
    public required MapNodeTemplate Second { get; init; }
}

public class MapTemplate
{
    public required List<MapNodeTemplate> Nodes { get; init; }
    public required List<MapNodeLinkTemplate> Adjacent { get; init; }

    public MapNodeTemplate GetSpawnNode(int spawnNumber)
    {
        return Nodes.First(node => node.SpawnNumber == spawnNumber);
    }
}