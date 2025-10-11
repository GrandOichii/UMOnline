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

    public static MapTemplate BuildDefault()
    {
        // 00 - 01
        // |  X  |
        // 10 - 11
        var node00 = new MapNodeTemplate()
        {
            Id = 0,
            Zones = [0],
            SpawnNumber = 0,
        };
        var node01 = new MapNodeTemplate()
        {
            Id = 1,
            Zones = [0],
            SpawnNumber = 1,
        };
        var node10 = new MapNodeTemplate()
        {
            Id = 2,
            Zones = [0],
            SpawnNumber = 2,
        };
        var node11 = new MapNodeTemplate()
        {
            Id = 3,
            Zones = [0],
            SpawnNumber = 3,
        };

        return new()
        {
            Nodes = [node00, node01, node10, node11],
            Adjacent = [
                .. Bidirectional(node00, node01),
                .. Bidirectional(node00, node10),
                .. Bidirectional(node00, node11),

                .. Bidirectional(node01, node10),
                .. Bidirectional(node01, node11),

                .. Bidirectional(node10, node11),
            ]
        };
    }
}