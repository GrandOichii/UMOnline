using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Cards;
using UMCore.Templates;

namespace UMCore.Matches;

public class Fighter
{
    public FighterTemplate Template { get; }
    public Player Owner { get; }
    public Match Match { get; }
    public string Name { get; private set; }
    public Health Health { get; }

    public string LogName => $"({Owner.Idx}){GetName()}({(Template.IsHero ? 'h' : 's')})";

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
        // if the fighter is not on the board they do not count as "Alive" (Invisible Man)
        return
            !Health.IsDead &&
            Match.Map.GetFighterLocation(this) is not null;
    }

    public async Task<int> ProcessDamage(int amount)
    {
        var dealt = Health.DealDamage(amount);
        Match.Logger?.LogDebug("{FighterLogName} is dealt {Amount} damage (original amount: {OriginalAmount})", LogName, dealt, amount);

        // TODO check for death
        // TODO update clients

        return dealt;
    }

    public async Task<int> RecoverHealth(int amount)
    {
        var recovered = Health.Recover(amount);
        Match.Logger?.LogDebug("{FighterLogName} recovers {Amount} damage (original amount: {OriginalAmount})", LogName, recovered, amount);

        // TODO update clients

        return recovered;
    }

    public IEnumerable<MatchCard> GetValidAttackCards()
    {
        return Owner.Hand.Cards.Where(c => c.CanBeUsedAsAttack(this));
    }

    public IEnumerable<MatchCard> GetValidDefenceCards()
    {
        return Owner.Hand.Cards.Where(c => c.CanBeUsedAsDefence(this));
    }

    public IEnumerable<Fighter> GetReachableFighters()
    {
        // TODO get targets within reach
        var range = 1; // TODO some characters have increased range
        IEnumerable<Fighter> result = [.. Match.Map.GetReachableFighters(this, range)];

        if (Template.IsRanged)
        {
            // TODO add all fighters in zones
        }

        return result;
    }

    public async Task Defend()
    {
        if (Match.Combat is null)
        {
            throw new Exception($"Called {Defend} on fighter {LogName} while no combat is active");
        }

        var availableDefence = GetValidDefenceCards();
        var defence = await Owner.Controller.ChooseCardInHandOrNothing(Owner, Owner.Idx, availableDefence, "Choose defence card");
        if (defence is null)
        {
            Match.Logger?.LogDebug("Player {PlayerLogName} decides not to defend", Owner.LogName);
        }
        else
        {
            Match.Logger?.LogDebug("Player {PlayerLogName} places a defence card", Owner.LogName);
            await Owner.Hand.Remove(defence);
        }

        await Match.Combat.SetDefenceCard(defence);
    }

    public bool IsInZone(IEnumerable<int> zones)
    {
        if (!IsAlive()) return false;
        var node = Match.Map.GetFighterLocation(this);
        // TODO check for null
        return node!.IsInZone(zones);
    }
}

public class Health(Fighter fighter)
{
    public int Current { get; private set; } = fighter.Template.StartingHealth;
    public int Max { get; private set; } = fighter.Template.MaxHealth;
    public bool IsDead => Current == 0;

    public int DealDamage(int amount)
    {
        var old = Current;
        Current -= amount;
        if (Current < 0)
        {
            Current = 0;
        }
        return old - Current;
    }

    public int Recover(int amount)
    {
        var old = Current;
        Current += amount;
        if (Current > Max) Current = Max;

        return Current - old;
    }
}