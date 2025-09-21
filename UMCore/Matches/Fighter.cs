using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Players;
using UMCore.Templates;

namespace UMCore.Matches;

public class Fighter
{
    public FighterTemplate Template { get; }
    public Player Owner { get; }
    public Match Match { get; }
    public string Name { get; private set; }
    public Health Health { get; }

    public string LogName => $"{GetName()}({(Template.IsHero ? 'h' : 's')})";

    public Fighter(Player owner, FighterTemplate template)
    {
        Template = template;
        Owner = owner;
        Match = owner.Match;
        Name = template.Name;

        Health = new(this);
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

    public async Task ProcessDamage(int amount)
    {
        Match.Logger?.LogDebug("{FighterLogName} processes {Amount} damage", LogName, amount);

        // TODO
        await Health.DealDamage(amount);
    }
}

public class Health(Fighter fighter)
{
    public int Current { get; private set; } = fighter.Template.StartingHealth;
    public int Max { get; private set; } = fighter.Template.MaxHealth;

    public async Task DealDamage(int amount)
    {
        Current -= amount;
        if (Current < 0)
        {
            // TODO death
            Current = 0;
        }

        // TODO update clients
    }
}