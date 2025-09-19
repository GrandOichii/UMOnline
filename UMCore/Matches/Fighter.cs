using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches;

public class Fighter
{
    public bool Hero { get; init; }
    public Player Owner { get; }
    public Match Match { get; }

    public Fighter(Player owner)
    {
        Owner = owner;
        Match = owner.Match;
    }

    public bool IsHero()
    {
        return Hero;
    }

    public bool IsSidekick()
    {
        return !Hero;
    }

    public MapNodeTemplate? Position { get; set; } = null;

    public int Movement()
    {
        // TODO
        return 2;
    }
}