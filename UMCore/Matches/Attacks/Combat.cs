using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Logging;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;

namespace UMCore.Matches.Attacks;

public class CombatPart : IHasData<CombatPart.Data>
{
    public bool IsDefence { get; }
    public MatchCard Card { get; }
    public int Value { get; set; }
    public bool EffectsCancelled { get; private set; } = false;
    public List<MatchCard> Boosts { get; } = [];
    public Combat Parent { get; }
    public Fighter Fighter { get; }

    public CombatPart(Combat parent, Fighter fighter, MatchCard card, bool isDefence)
    {
        Fighter = fighter;
        IsDefence = isDefence;
        Card = card;
        Parent = parent;
        Value = (int)card.Template.Value!;
    }

    public async Task ExecuteCombatCardChoiceEffects()
    {
        var effects = Parent.Match.GetEffectCollectionThatAccepts(new(this), f => f.OnCombatCardChoiceEffects);

        // TODO order effects
        foreach (var (source, effect) in effects)
            effect.Execute(new(source), new(this));
    }

    public void ApplyModifiers()
    {
        var subs = new Effects.EffectCollectionSubjects(Fighter, null, this);
        var modifiers = Parent.Match.GetEffectCollectionThatAccepts(subs, f => f.CardValueModifiers);
        foreach (var (source, modifier) in modifiers)
        {
            Value = modifier.Modify(new(source), subs, Value);
        }
    }

    public int GetValue()
    {
        var result = Value;
        foreach (var boost in Boosts)
        {
            result += (int)boost.GetBoostValue()!;
        }
        return result;
    }

    public async Task CancelEffects()
    {
        EffectsCancelled = true;
        Parent.Match.Logs.Public($"Effects of card {Card.FormattedLogName} are cancelled");

        await DiscardBoostCards();
    }

    public bool CanBeCancelled(Player byPlayer)
    {
        if (!Card.HasEffects()) return false;

        return Card.CanBeCancelled(byPlayer);
    }

    public async Task DiscardBoostCards()
    {
        foreach (var boost in Boosts)
            await boost.PlaceIntoDiscard();
        Boosts.Clear();
    }

    public async Task Discard()
    {
        await Card.PlaceIntoDiscard();
        await DiscardBoostCards();
    }

    public async Task AddBoost(MatchCard card)
    {
        Boosts.Add(card);
        await card.Owner.Match.UpdateClients();
    }

    public Data GetData(Player player)
    {
        var visible = Card.Owner == player || Parent.CardsAreRevealed;
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

    public CombatPart? AttackCard { get; private set; }
    public CombatPart? DefenceCard { get; private set; }

    public Player? Winner { get; private set; }
    public bool CardsAreRevealed { get; private set; } = false;
    public int? DamageDealt { get; private set; }

    public Combat(Match match, AvailableAttack original)
    {
        Match = match;
        Attacker = original.Fighter;
        Defender = original.Target;
        DamageDealt = null;
        AttackCard = new(this, Attacker, original.AttackCard, false);
    }

    public async Task SetDefenceCard(MatchCard? defence)
    {
        DefenceCard = defence is null ? null : new(this, Defender, defence, true);

        await Match.UpdateClients();
    }

    public Player Initiator => Attacker.Owner;

    public async Task EmitTrigger(CombatStepTrigger trigger)
    {
        // cards
        List<(CombatPart?, Fighter)> cards = [(DefenceCard, Defender), (AttackCard, Attacker)];
        foreach (var (card, fighter) in cards)
        {
            if (card is null) continue;
            
            List<CombatStepEffectsCollection> effects = [
                fighter.CombatStepEffects,
            ];
            
            if (!card.EffectsCancelled)
                effects.Add(card.Card.CombatStepEffects);

            foreach (var effect in effects)
            {
                await effect.Execute(trigger, fighter);
                await Match.UpdateClients();
            }
            // await .Execute(trigger, fighter);
            if (Match.IsWinnerDetermined()) return;
        }
    }

    public async Task Process()
    {
        await Initiator.Hand.Remove(AttackCard!.Card);
        await Match.UpdateClients();

        await AttackCard.ExecuteCombatCardChoiceEffects();

        await Initiator.ExecuteOnAttackEffects(Attacker);

        await Defender.Defend();
        await Match.UpdateClients();

        CardsAreRevealed = true;
        var logMsg = $"Combat cards are revealed! Attacker {Attacker.Owner.FormattedLogName} played {AttackCard.Card.FormattedLogName}";
        logMsg += DefenceCard is null
            ? ""
            : $", {Defender.Owner.FormattedLogName} played {DefenceCard.Card.FormattedLogName}";
        Match.Logs.Public(logMsg);

        // execute "immediately"
        await EmitTrigger(CombatStepTrigger.Immediately);
        if (Match.IsWinnerDetermined()) return;

        // before "during combat", apply card value modifiers
        AttackCard.ApplyModifiers();
        DefenceCard?.ApplyModifiers();
        
        // execute "during combat"
        await EmitTrigger(CombatStepTrigger.DuringCombat);
        if (Match.IsWinnerDetermined()) return;

        // deal damage
        var damage = AttackCard.GetValue();
        if (DefenceCard is not null)
        {
            damage -= DefenceCard.GetValue();
        }
        Match.Logger?.LogDebug("Attacker value: {AttackerValue}, defender value: {DefenderValue}", AttackCard.GetValue(), DefenceCard?.GetValue() ?? 0);
        if (damage < 0) damage = 0;
        DamageDealt = await Defender.ProcessDamage(damage, true);
        if (Match.IsWinnerDetermined()) return;
        Winner = DamageDealt > 0
            ? Attacker.Owner
            : Defender.Owner;

        // execute "after combat"
        await EmitTrigger(CombatStepTrigger.AfterCombat);
        if (Match.IsWinnerDetermined()) return;

        // discard cards
        Match.Logger?.LogDebug("Discarding combat cards");
        Match.Logs.Public("Discarding combat cards");

        if (AttackCard is not null)
            await AttackCard.Discard();

        if (DefenceCard is not null)
            await DefenceCard.Discard();

        await Initiator.ExecuteAfterAttackEffects(Attacker);
    }

    public (CombatPart?, Fighter) GetOpponent(CombatPart? part)
    {
        if (part == AttackCard) return (DefenceCard, Defender);
        if (part == DefenceCard) return (AttackCard, Attacker);

        throw new MatchException($"Stale CombatPart passed to {nameof(GetOpponent)}");
    }

    public async Task CancelEffectsOfOpponent(Player player)
    {
        var (card, fighter, _) = GetCombatPart(player);
        var (oppCard, _) = GetOpponent(card);
        if (oppCard is null) return;
        if (!oppCard.CanBeCancelled(player)) return;

        await oppCard.CancelEffects();
        Match.Logger?.LogDebug("Effects of card {CardLogName} of player {PlayerLogName} were cancelled", oppCard.Card.LogName, oppCard.Card.Owner.LogName);
        await Match.UpdateClients();
    }

    public (CombatPart?, Fighter, bool isDefense) GetCombatPart(Player player) {
        if (Defender.Owner == player)
        {
            return (DefenceCard, Defender, true);
        }
        if (Attacker.Owner == player)
        {
            return (AttackCard, Attacker, false);
        }

        throw new MatchException($"Failed to find combat part for player {player.LogName}");
    }

    public (CombatPart?, Fighter) RemoveCombatPart(Player player) {
        if (Defender.Owner == player)
        {
            var result = DefenceCard;
            DefenceCard = null;
            return (result, Defender);
        }
        if (Attacker.Owner == player)
        {
            var result = AttackCard;
            AttackCard = null;
            return (result, Defender);
        }

        throw new MatchException($"Failed to find combat part for player {player.LogName}");
    }

    public async Task AddBoostToPlayer(Player player, MatchCard boostCard)
    {
        var (card, fighter, _) = GetCombatPart(player);
        if (card is null)
        {
            throw new MatchException($"Cannot boost empty card");
        }
        await card.AddBoost(boostCard);
    }

    public Player? GetLoser()
    {
        if (Winner == null) return null;
        if (Attacker.Owner == Winner) return Defender.Owner;
        if (Defender.Owner == Winner) return Attacker.Owner;
        throw new MatchException($"Something went very wrong in combat: Winner is not attacker nor defender when calling {GetLoser} ");
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Attacker = Attacker.GetData(player),
            AttackCard = AttackCard?.GetData(player),
            Defender = Defender.GetData(player),
            DefenceCard = DefenceCard?.GetData(player),
            WinnerIdx = Winner?.Idx,
        };
    }

    public class Data
    {
        public required Fighter.Data Attacker { get; init; }
        public required CombatPart.Data? AttackCard { get; init; }

        public required Fighter.Data Defender { get; init; }
        public required CombatPart.Data? DefenceCard { get; init; }

        public required int? WinnerIdx { get; init; }
    }
}