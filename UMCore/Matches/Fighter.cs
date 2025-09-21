using System.Diagnostics.Contracts;
using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches;

public class Fighter
{
    public FighterTemplate Template { get; }
    public Player Owner { get; }
    public Match Match { get; }
    public string Name { get; private set; }

    public string LogName => $"{GetName()}({(Template.IsHero ? 'h' : 's')})";

    public Fighter(Player owner, FighterTemplate template)
    {
        Template = template;
        Owner = owner;
        Match = owner.Match;
        Name = template.Name;
    }
    
    public string GetName()
    {
        return Name;
    }

    public bool IsHero()
    {
        return Template.IsHero;
    }

    public bool IsSidekick()
    {
        return !Template.IsHero;
    }

    public int Movement()
    {
        return Template.Movement;
    }

    public bool IsOpposingTo(Player player)
    {
        // TODO change with teams
        return Owner != player;
    }

    public bool IsFriendlyTo(Player player)
    {
        // TODO change with teams
        return Owner == player;
    }

    public bool IsAlive()
    {
        // TODO
        return true;
    }
}