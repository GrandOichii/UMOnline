using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore.Matches.Attacks;

public class CombatCard(MatchCard card)
{
    public MatchCard Card { get; } = card;
    public int Value = card.Card.Template.Value;
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
        // TODO first check and execute defender, then check and execute attacker
        await AttackCard.Card.ExecuteCombatStepTrigger(trigger, Attacker);
        if (DefenceCard is not null)
            await DefenceCard.Card.ExecuteCombatStepTrigger(trigger, Defender);
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

        // execute "after combat"
        await EmitTrigger(CombatStepTrigger.AfterCombat);

        // discard cards
        await AttackCard.Card.PlaceIntoDiscard();
        if (DefenceCard is not null)
            await DefenceCard.Card.PlaceIntoDiscard();
    }
}