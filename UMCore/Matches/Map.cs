using UMCore.Templates;

namespace UMCore.Matches;

public class Map
{
    public Match Match { get; }
    public MapTemplate Template { get; }

    public Map(Match match, MapTemplate template)
    {
        Match = match;
        Template = template;
    }

    public IEnumerable<MapNodeTemplate> Adjacent(MapNodeTemplate node)
    {
        // TODO dont know how this works with non-bidirectional connections

        // TODO? cache result
        return Template.Adjacent
            .Where(link => link.First == node)
            .Select(link => link.Second)
            .ToHashSet();
    }
}