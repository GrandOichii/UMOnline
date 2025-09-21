using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore.Matches.Attacks;

public class CombatCard(MatchCard card)
{
    public MatchCard Card { get; } = card;
    public int Value = card.Card.Template.Value;
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

    public async Task Process()
    {
        await Initiator.Hand.Remove(AttackCard.Card);

        // TODO update clients

        await Defender.Defend();

        // TODO update clients

        // TODO reveal cards

        // TODO execute "immediately"

        // TODO execute "during combat"

        // deal damage
        var damage = AttackCard.Value;
        if (DefenceCard is not null)
        {
            damage -= DefenceCard.Value;
        }
        if (damage < 0) damage = 0;
        await Defender.ProcessDamage(damage);

        // TODO execute "after combat"

        // discard cards
        await AttackCard.Card.PlaceIntoDiscard();
        if (DefenceCard is not null)
            await DefenceCard.Card.PlaceIntoDiscard();
    }
}