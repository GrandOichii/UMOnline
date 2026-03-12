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

	private static IEnumerable<MapNodeLinkTemplate> Bidirectional(MapNodeTemplate n1, MapNodeTemplate n2)
	{
		return [
			new() {
				First = n1.Id,
				Second = n2.Id,
			},
		];
	}

	public static MapTemplate GetBaskervilleTemplate()
	{
		List<MapNodeTemplate> nodes = [
			new() {
				Id = 0,
				Zones = [0],
				HasSecretPassage = true,
			},
			new() {
				Id = 1,
				Zones = [0],
			},
			new() {
				Id = 2,
				Zones = [0],
			},
			new() {
				Id = 3,
				Zones = [0],
			},
			new() {
				Id = 4,
				Zones = [0, 1],
				SpawnNumber = 2,
			},
			new() {
				Id = 5,
				Zones = [0, 6],
			},
			new() {
				Id = 6,
				Zones = [1],
			},
			new() {
				Id = 7,
				Zones = [1],
			},
			new() {
				Id = 8,
				Zones = [1],
			},
			new() {
				Id = 9,
				Zones = [1, 2, 3],
			},
			new() {
				Id = 10,
				Zones = [2],
			},
			new() {
				Id = 11,
				Zones = [2],
			},
			new() {
				Id = 12,
				Zones = [2],
				HasSecretPassage = true,
			},
			new() {
				Id = 13,
				Zones = [3, 4],
			},
			new() {
				Id = 14,
				Zones = [4],
			},
			new() {
				Id = 15,
				Zones = [4],
			},
			new() {
				Id = 16,
				Zones = [4],
				SpawnNumber = 1,
			},
			new() {
				Id = 17,
				Zones = [3],
				HasSecretPassage = true,
			},
			new() {
				Id = 18,
				Zones = [3],
			},
			new() {
				Id = 19,
				Zones = [3, 5],
				SpawnNumber = 3,
			},
			new() {
				Id = 20,
				Zones = [5],
			},
			new() {
				Id = 21,
				Zones = [5],
			},
			new() {
				Id = 22,
				Zones = [4, 5],
			},
			new() {
				Id = 23,
				Zones = [5],
			},
			new() {
				Id = 24,
				Zones = [5],
			},
			new() {
				Id = 25,
				Zones = [5],
			},
			new() {
				Id = 26,
				Zones = [5],
			},
			new() {
				Id = 27,
				Zones = [5],
				HasSecretPassage = true,
			},
			new() {
				Id = 28,
				Zones = [5, 6],
			},
			new() {
				Id = 29,
				Zones = [3, 6],
			},
			new() {
				Id = 30,
				Zones = [6],
			},
			new() {
				Id = 31,
				Zones = [6],
				SpawnNumber = 4,
			},
		];
		return new()
		{
			Nodes = nodes,
			Adjacent = [
				.. Bidirectional(nodes[0], nodes[2]),
				.. Bidirectional(nodes[0], nodes[1]),
				.. Bidirectional(nodes[4], nodes[1]),
				.. Bidirectional(nodes[4], nodes[5]),
				.. Bidirectional(nodes[4], nodes[6]),
				.. Bidirectional(nodes[7], nodes[6]),
				.. Bidirectional(nodes[7], nodes[8]),
				.. Bidirectional(nodes[7], nodes[9]),
				.. Bidirectional(nodes[8], nodes[9]),
				.. Bidirectional(nodes[10], nodes[9]),
				.. Bidirectional(nodes[11], nodes[9]),
				.. Bidirectional(nodes[13], nodes[9]),
				.. Bidirectional(nodes[13], nodes[14]),
				.. Bidirectional(nodes[15], nodes[14]),
				.. Bidirectional(nodes[15], nodes[16]),
				.. Bidirectional(nodes[13], nodes[16]),
				.. Bidirectional(nodes[13], nodes[17]),
				.. Bidirectional(nodes[13], nodes[21]),
				.. Bidirectional(nodes[22], nodes[21]),
				.. Bidirectional(nodes[22], nodes[23]),
				.. Bidirectional(nodes[22], nodes[15]),
				.. Bidirectional(nodes[21], nodes[23]),
				.. Bidirectional(nodes[18], nodes[17]),
				.. Bidirectional(nodes[11], nodes[10]),
				.. Bidirectional(nodes[12], nodes[10]),
				.. Bidirectional(nodes[13], nodes[20]),
				.. Bidirectional(nodes[21], nodes[20]),
				.. Bidirectional(nodes[19], nodes[20]),
				.. Bidirectional(nodes[19], nodes[24]),
				.. Bidirectional(nodes[19], nodes[18]),
				.. Bidirectional(nodes[29], nodes[18]),
				.. Bidirectional(nodes[29], nodes[28]),
				.. Bidirectional(nodes[27], nodes[28]),
				.. Bidirectional(nodes[27], nodes[26]),
				.. Bidirectional(nodes[25], nodes[26]),
				.. Bidirectional(nodes[25], nodes[19]),
				.. Bidirectional(nodes[25], nodes[24]),
				.. Bidirectional(nodes[28], nodes[30]),
				.. Bidirectional(nodes[31], nodes[30]),
				.. Bidirectional(nodes[2], nodes[30]),
				.. Bidirectional(nodes[31], nodes[5]),
				.. Bidirectional(nodes[24], nodes[20]),
				.. Bidirectional(nodes[6], nodes[8]),
				.. Bidirectional(nodes[3], nodes[5]),
				.. Bidirectional(nodes[3], nodes[2]),
			]
		};
	}

}