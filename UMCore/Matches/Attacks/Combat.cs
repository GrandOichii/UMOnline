using Microsoft.Extensions.Logging;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore.Matches.Attacks;

public class CombatCard(MatchCard card)
{
    public MatchCard Card { get; } = card;
    public int Value { get; set; } = card.Card.Template.Value;
    public bool EffectsCancelled { get; private set; } = false;

    public void CancelEffects()
    {
        EffectsCancelled = true;
    }

    public bool CanBeCancelled()
    {
        // TODO check if can be cancelled

        return true;        
    }
}


public enum CombatStepTrigger {
    Immediately = 0,
    DuringCombat = 1,
    AfterCombat = 2,
}

public class Combat
{
    public Match Match { get; }

    public Fighter Attacker { get; private set; }
    public Fighter Defender { get; private set; }

    public CombatCard AttackCard { get; private set; }
    public CombatCard? DefenceCard { get; private set; }

    public Player? Winner { get; private set; }

    public Combat(Match match, AvailableAttack original)
    {
        Match = match;
        Attacker = original.Fighter;
        Defender = original.Target;
        AttackCard = new(original.AttackCard);
    }

    public async Task SetDefenceCard(MatchCard? defence)
    {
        DefenceCard = defence is null ? null : new(defence);

        // TODO update clients
    }

    public Player Initiator => Attacker.Owner;

    public async Task EmitTrigger(CombatStepTrigger trigger)
    {
        List<(CombatCard?, Fighter)> cards = [(DefenceCard, Defender), (AttackCard, Attacker)];
        foreach (var (card, fighter) in cards)
        {
            if (card is null) continue;
            if (card.EffectsCancelled) continue;
            await card.Card.ExecuteCombatStepTrigger(trigger, fighter);
        }
    }

    public async Task Process()
    {
        await Initiator.Hand.Remove(AttackCard.Card);

        // TODO update clients

        await Defender.Defend();

        // TODO update clients

        // TODO reveal cards

        // execute "immediately"
        await EmitTrigger(CombatStepTrigger.Immediately);

        // execute "during combat"
        await EmitTrigger(CombatStepTrigger.DuringCombat);

        // deal damage
        var damage = AttackCard.Value;
        if (DefenceCard is not null)
        {
            damage -= DefenceCard.Value;
        }
        if (damage < 0) damage = 0;
        await Defender.ProcessDamage(damage);
        Winner = damage > 0
            ? Attacker.Owner
            : Defender.Owner;

        // execute "after combat"
        await EmitTrigger(CombatStepTrigger.AfterCombat);

        // discard cards
        await AttackCard.Card.PlaceIntoDiscard();
        if (DefenceCard is not null)
            await DefenceCard.Card.PlaceIntoDiscard();
    }

    public async Task CancelEffectsOfOpponent(Player player)
    {
        if (Attacker.Owner == player)
        {
            if (DefenceCard is null) return;
            if (!DefenceCard.CanBeCancelled()) return;
            DefenceCard.CancelEffects();
            Match.Logger?.LogDebug("Effects of defence card {CardLogName} of player {PlayerLogName} were cancelled", DefenceCard.Card.LogName, Defender.Owner.LogName);
            // TODO update clients
            return;
        }

        // TODO assert that Defender.Owner == player
        // is defender
        if (!AttackCard.CanBeCancelled()) return;

        AttackCard.CancelEffects();
        Match.Logger?.LogDebug("Effects of attack card {CardLogName} of player {PlayerLogName} were cancelled", AttackCard.Card.LogName, Attacker.Owner.LogName);

        // TODO update clients
    }
}