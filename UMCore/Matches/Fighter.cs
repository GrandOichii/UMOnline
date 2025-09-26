using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Cards;
using UMCore.Matches.Effects;
using UMCore.Matches.Players;
using UMCore.Matches.Players.Cards;
using UMCore.Templates;
using UMCore.Utility;

namespace UMCore.Matches;

public class Fighter : IHasData<Fighter.Data>, IHasSetupData<Fighter.SetupData>
{
    public int Id { get; }
    public FighterTemplate Template { get; }
    public Player Owner { get; }
    public Match Match { get; }
    public string Name { get; private set; }
    public Health Health { get; }
    public Dictionary<TurnPhaseTrigger, EffectCollection> TurnPhaseEffects { get; }

    public string LogName => $"({Owner.Idx}){GetName()}({(Template.IsHero ? 'h' : 's')})";

    public string FormattedLogName => $"{GetName()}"; // TODO

    public Fighter(Player owner, FighterTemplate template)
    {
        Template = template;
        Owner = owner;
        Match = owner.Match;
        Name = template.Name;

        Id = Match.AddFighter(this);

        Health = new(this);

        // effects
        LuaTable data;
        try
        {
            Match.LState.DoString(template.Script);
            var creationFunc = LuaUtility.GetGlobalF(Match.LState, "_Create");
            var returned = creationFunc.Call();
            data = LuaUtility.GetReturnAs<LuaTable>(returned);
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to run fighter creation function in fighter {template.Name}", e); // TODO type
        }

        TurnPhaseEffects = [];
        try
        {
            var turnPhaseEffects = LuaUtility.TableGet<LuaTable>(data, "TurnPhaseEffects");
            foreach (var keyRaw in turnPhaseEffects.Keys)
            {
                var key = (TurnPhaseTrigger)Convert.ToInt32(keyRaw);
                var table = turnPhaseEffects[keyRaw] as LuaTable;
                // TODO check for null
                var effects = new EffectCollection(table!);
                TurnPhaseEffects.Add(key, effects);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to get turn step effects for fighter {template.Name}", e); // TODO type
        }

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
        return GetStatus() == FighterStatus.Alive;
    }

    public FighterStatus GetStatus()
    {
        if (Health.IsDead)
        {
            return FighterStatus.Dead;
        }
        if (Match.Map.GetFighterLocation(this) is null)
        {
            return FighterStatus.OffBoard;
        }
        return FighterStatus.Alive;
    }

    public async Task<int> ProcessDamage(int amount, bool isCombatDamage = false)
    {
        var dealt = Health.DealDamage(amount);
        Match.Logger?.LogDebug("{FighterLogName} is dealt {Amount} damage (original amount: {OriginalAmount})", LogName, dealt, amount);
        Match.Logs.Public($"Fighter {FormattedLogName} is dealt {amount} damage");

        // TODO check for death
        if (!IsAlive())
        {
            Match.Logs.Public($"Fighter {FormattedLogName} dies!");
            Match.Logger?.LogDebug("Fighter {FighterLogName} dies", LogName);
            if (!Owner.GetAliveFighters().Select(f => f.IsHero()).Any())
            {
                throw new Exception("MATCH ENDED");
            }
            await Match.Map.RemoveFighterFromBoard(this);
        }
        await Match.UpdateClients();

        return dealt;
    }

    public async Task<int> RecoverHealth(int amount)
    {
        var recovered = Health.Recover(amount);
        Match.Logger?.LogDebug("{FighterLogName} recovers {Amount} damage (original amount: {OriginalAmount})", LogName, recovered, amount);
        Match.Logs.Public($"Fighter {FormattedLogName} recovers {recovered} health");

        await Match.UpdateClients();

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
        List<Fighter> result = [.. Match.Map.GetReachableFighters(this, range)];

        if (Template.IsRanged)
        {
            result.AddRange(Match.Map.GetRangedReachableFighters(this));
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
        var defence = await Owner.Controller.ChooseCardInHandOrNothing(Owner, Owner.Idx, [.. availableDefence], "Choose defence card");
        if (defence is null)
        {
            Match.Logger?.LogDebug("Player {PlayerLogName} decides not to defend", Owner.LogName);
            Match.Logs.Public($"Player {Owner.FormattedLogName} decides not to defend");

        }
        else
        {
            Match.Logs.Private(
                Owner,
                $"You use {defence.FormattedLogName} as defence",
                $"Player {Owner.FormattedLogName} places a defend card");

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

    public IEnumerable<EffectCollection> GetTurnPhaseEffects(TurnPhaseTrigger trigger)
    {
        if (TurnPhaseEffects.TryGetValue(trigger, out EffectCollection? value))
            return [value];
        return [];
    }

    public Data GetData(Player player)
    {
        return new()
        {
            Id = Id,
            Name = GetName(),
            IsAlive = IsAlive(),
            CurHealth = Health.Current,
            MaxHealth = Health.Max,
        };
    }

    public SetupData GetSetupData()
    {
        return new()
        {
            Id = Id,
            Key = Template.Key
        };
    }

    public class Data
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required bool IsAlive { get; init; }
        public required int CurHealth { get; init; }
        public required int MaxHealth { get; init; }
    }

    public class SetupData
    {
        public required int Id { get; init; }
        public required string Key { get; init; }
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

public enum FighterStatus
{
    Alive = 0,
    Dead = 1,
    OffBoard = 2,
}