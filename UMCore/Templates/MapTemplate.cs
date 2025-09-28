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
    public required int First { get; init; }
    public required int Second { get; init; }
    public bool Bidirectional { get; init; } = true;
}

public class MapTemplate
{
    public required List<MapNodeTemplate> Nodes { get; init; }
    public required List<MapNodeLinkTemplate> Adjacent { get; init; }

    public MapNodeTemplate GetSpawnNode(int spawnNumber)
    {
        return Nodes.First(node => node.SpawnNumber == spawnNumber);
    }

    public MapNodeTemplate GetNode(int id) => Nodes.Single(n => n.Id == id);
}