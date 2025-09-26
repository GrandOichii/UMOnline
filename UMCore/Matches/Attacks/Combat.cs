using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore.Matches.Attacks;

public class CombatCard(Combat parent, MatchCard card) : IHasData<CombatCard.Data>
{
    public MatchCard Card { get; } = card;
    public int Value { get; set; } = card.Template.Value;
    public bool EffectsCancelled { get; private set; } = false;
    public List<MatchCard> Boosts { get; } = [];

    public int GetValue()
    {
        var result = Value;
        foreach (var boost in Boosts)
            result += boost.GetBoostValue();
        return result;
    }

    public void CancelEffects()
    {
        EffectsCancelled = true;
    }

    public bool CanBeCancelled()
    {
        // TODO check if can be cancelled

        return true;
    }

    public async Task Discard()
    {
        await Card.PlaceIntoDiscard();
        foreach (var boost in Boosts)
            await boost.PlaceIntoDiscard();
    }

    public async Task AddBoost(MatchCard card)
    {
        Boosts.Add(card);
        await card.Owner.Match.UpdateClients();
    }

    public Data GetData(Player player)
    {
        var visible = Card.Owner == player || parent.Winner is not null;
        if (visible) {
            return new()
            {
                Value = Value,
                DeckName = Card.Owner.Loadout.Name,
                Card = Card.GetData(player),
                Boosts = [.. Boosts.Select(b => b.GetData(player))],
            };
        }
        return new()
        {
            Value = null,
            Card = null,
            DeckName = Card.Owner.Loadout.Name,
            Boosts = [.. Boosts.Select<MatchCard, MatchCard.Data?>(b => null)],
        };
    }

    public class Data
    {
        public required int? Value { get; init; }
        public required MatchCard.Data? Card { get; init; }
        public required MatchCard.Data?[] Boosts { get; init; }
        public required string DeckName { get; init; }
    }
}


public enum CombatStepTrigger {
    Immediately = 0,
    DuringCombat = 1,
    AfterCombat = 2,
}

public class Combat : IHasData<Combat.Data>
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
        AttackCard = new(this, original.AttackCard);
    }

    public async Task SetDefenceCard(MatchCard? defence)
    {
        DefenceCard = defence is null ? null : new(this, defence);

        await Match.UpdateClients();
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
            await Match.UpdateClients();
        }
    }

    public async Task Process()
    {
        await Initiator.Hand.Remove(AttackCard.Card);
        await Match.UpdateClients();

        await Defender.Defend();
        await Match.UpdateClients();

        // TODO reveal cards

        // execute "immediately"
        await EmitTrigger(CombatStepTrigger.Immediately);

        // execute "during combat"
        await EmitTrigger(CombatStepTrigger.DuringCombat);

        // deal damage
        var damage = AttackCard.GetValue();
        if (DefenceCard is not null)
        {
            damage -= DefenceCard.GetValue();
        }
        if (damage < 0) damage = 0;
        await Defender.ProcessDamage(damage);
        Winner = damage > 0
            ? Attacker.Owner
            : Defender.Owner;

        // execute "after combat"
        await EmitTrigger(CombatStepTrigger.AfterCombat);

        // discard cards
        Match.Logger?.LogDebug("Discarding combat cards");
        await AttackCard.Discard();

        if (DefenceCard is not null)
            await DefenceCard.Discard();
    }

    public async Task CancelEffectsOfOpponent(Player player)
    {
        if (Attacker.Owner == player)
        {
            if (DefenceCard is null) return;
            if (!DefenceCard.CanBeCancelled()) return;
            DefenceCard.CancelEffects();
            Match.Logger?.LogDebug("Effects of defence card {CardLogName} of player {PlayerLogName} were cancelled", DefenceCard.Card.LogName, Defender.Owner.LogName);
            await Match.UpdateClients();

            return;
        }

        // TODO assert that Defender.Owner == player
        // is defender
        if (!AttackCard.CanBeCancelled()) return;

        AttackCard.CancelEffects();
        Match.Logger?.LogDebug("Effects of attack card {CardLogName} of player {PlayerLogName} were cancelled", AttackCard.Card.LogName, Attacker.Owner.LogName);

        await Match.UpdateClients();

    }

    public (CombatCard?, Fighter) GetCombatPart(Player player) {
        if (Defender.Owner == player)
        {
            return (DefenceCard, Defender);
        }
        if (Attacker.Owner == player)
        {
            return (AttackCard, Attacker);
        }

        // TODO throw exception
        throw new Exception();
    }

    public async Task AddBoostToPlayer(Player player, MatchCard boostCard)
    {
        var (card, fighter) = GetCombatPart(player);
        if (card is null)
        {
            // TODO throw exception
            return;
        }
        await card.AddBoost(boostCard);
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Attacker = Attacker.GetData(player),
            AttackCard = AttackCard.GetData(player),
            Defender = Defender.GetData(player),
            DefenceCard = DefenceCard?.GetData(player),
            WinnerIdx = Winner?.Idx,
        };
    }

    public class Data
    {
        public required Fighter.Data Attacker { get; init; }
        public required CombatCard.Data AttackCard { get; init; }

        public required Fighter.Data Defender { get; init; }
        public required CombatCard.Data? DefenceCard { get; init; }

        public required int? WinnerIdx { get; init; }
    }
}