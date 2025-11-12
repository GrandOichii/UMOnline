using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Logging;
using NLua;
using UMCore.Matches.Cards;
using UMCore.Matches.Effects;
using UMCore.Matches.Fighters;
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

    public string FormattedLogName => $"[{Id}:{GetName()}]";

    public List<EffectCollection> CardValueModifiers { get; }
    public List<EffectCollection> WhenPlacedEffects { get; }
    public List<ManoeuvreValueModifier> ManoeuvreValueMods { get; }
    public List<EffectCollection> OnAttackEffects { get; }
    public List<EffectCollection> AfterAttackEffects { get; }
    public List<EffectCollection> AfterSchemeEffects { get; }
    public List<EffectCollection> GameStartEffects { get; }
    public List<MovementNodeConnection> MovementNodeConnections { get; }
    public List<CardCancellingForbid> CardCancellingForbids { get; }
    public List<EffectCollection> OnManoeuvreEffects { get; }
    public List<EffectCollection> OnDamageEffects { get; }
    public List<EffectCollection> OnFighterDefeatEffects { get; }
    public CombatStepEffectsCollection CombatStepEffects { get; }
    public List<DamageModifier> DamageModifiers { get; }
    public List<EffectCollection> AfterMovementEffects { get; }
    public List<BoostedMovementReplacer> BoostedMovementReplacers { get; }
    public List<OnMoveEffect> OnMoveEffects { get; }
    public List<ManoeuvreDrawAmountModifier> ManoeuvreDrawAmountModifiers { get; }
    public List<CombatResolutionEffect> OnLostCombatEffects { get; }
    public List<EffectCollection> OnCombatCardChoiceEffects { get; }
    public List<EffectCollection> WhenManoeuvreEffects { get; }

    public static List<LuaFunction> ExtractFunctionList(Fighter fighter, LuaTable data, string key)
    {
        try
        {
            List<LuaFunction> result = [];
            var table = LuaUtility.TableGet<LuaTable>(data, key);
            foreach (var value in table.Values)
            {
                var func = value as LuaFunction;
                // TODO check for null
                result.Add(func!);
            }
            return result;
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to extract function list {key} for fighter {fighter.Name}", e);
        }
    }

    public static List<LuaTable> ExtractTableList(Fighter fighter, LuaTable data, string key)
    {
        try
        {
            List<LuaTable> result = [];
            var table = LuaUtility.TableGet<LuaTable>(data, key);
            foreach (var value in table.Values)
            {
                var t = value as LuaTable;
                // TODO check for null
                result.Add(t!);
            }
            return result;
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to extract table list {key} for fighter {fighter.Name}", e);
        }
    }

    public static List<EffectCollection> ExtractEffectCollectionList(Fighter fighter, LuaTable data, string key)
    {
        try
        {
            List<EffectCollection> result = [];
            var table = LuaUtility.TableGet<LuaTable>(data, key);
            foreach (var value in table.Values)
            {
                var t = value as LuaTable;
                // TODO check for null
                result.Add(new(t!));
            }
            return result;
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to extract table list {key} for fighter {fighter.Name}", e);
        }
    }

    public Fighter(Player owner, FighterTemplate template)
    {
        Template = template;
        Owner = owner;
        Match = owner.Match;
        Name = template.Name;

        Id = Match.AddFighter(this);

        Health = new(this);

        // TODO this is ugly

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
            throw new MatchException($"Failed to run fighter creation function in fighter {template.Name}", e);
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
            throw new MatchException($"Failed to get turn step effects for fighter {template.Name}", e);
        }

        CardValueModifiers = ExtractEffectCollectionList(this, data, "CardValueModifiers");

        WhenPlacedEffects = ExtractEffectCollectionList(this, data, "WhenPlacedEffects");

        ManoeuvreValueMods = [ ..ExtractTableList(this, data, "ManoeuvreValueMods")
            .Select(t =>
                new ManoeuvreValueModifier(this, t)
            )
        ];

        OnAttackEffects = ExtractEffectCollectionList(this, data, "OnAttackEffects");
        AfterAttackEffects = ExtractEffectCollectionList(this, data, "AfterAttackEffects");
        AfterSchemeEffects = ExtractEffectCollectionList(this, data, "AfterSchemeEffects");
        WhenManoeuvreEffects = ExtractEffectCollectionList(this, data, "WhenManoeuvreEffects");

        GameStartEffects = ExtractEffectCollectionList(this, data, "GameStartEffects");

        MovementNodeConnections = [ ..ExtractFunctionList(this, data, "MovementNodeConnections")
            .Select(f =>
                new MovementNodeConnection(this, f)
            )
        ];

        CardCancellingForbids = [ ..ExtractFunctionList(this, data, "CardCancellingForbids")
            .Select(f =>
                new CardCancellingForbid(this, f)
            )
        ];

        OnManoeuvreEffects = ExtractEffectCollectionList(this, data, "OnManoeuvreEffects");
        OnDamageEffects = ExtractEffectCollectionList(this, data, "OnDamageEffects");

        OnFighterDefeatEffects = ExtractEffectCollectionList(this, data, "OnFighterDefeatEffects");

        // combat step effects
        try
        {
            CombatStepEffects = new(data);
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get combat step effects for fighter {Name}", e);
        }

        DamageModifiers = [ ..ExtractFunctionList(this, data, "DamageModifiers")
            .Select(f =>
                new DamageModifier(this, f)
            )
        ];

        AfterMovementEffects = ExtractEffectCollectionList(this, data, "AfterMovementEffects");
        OnCombatCardChoiceEffects = ExtractEffectCollectionList(this, data, "OnCombatCardChoiceEffects");

        BoostedMovementReplacers = [ ..ExtractFunctionList(this, data, "BoostedMovementReplacers")
            .Select(f =>
                new BoostedMovementReplacer(this, f)
            )
        ];

        OnMoveEffects = [ ..ExtractTableList(this, data, "OnMoveEffects")
            .Select(t =>
                new OnMoveEffect(this, t)
            )
        ];

        ManoeuvreDrawAmountModifiers = [ ..ExtractFunctionList(this, data, "ManoeuvreDrawAmountModifiers")
            .Select(f =>
                new ManoeuvreDrawAmountModifier(this, f)
            )
        ];

        OnLostCombatEffects = [ ..ExtractTableList(this, data, "OnLostCombatEffects")
            .Select(f =>
                new CombatResolutionEffect(this, f)
            )
        ];

        // tokens
        try
        {
            var tokenDeclarations = LuaUtility.TableGet<LuaTable>(data, "Tokens");
            foreach (string tokenName in tokenDeclarations.Keys)
            {
                var table = tokenDeclarations[tokenName] as LuaTable;
                // TODO check for null
                Match.Tokens.Declare(tokenName, this, table!);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get token declarations for fighter {template.Name}", e);
        }


    }
    
    public void ExecuteGameStartEffects()
    {
        foreach (var effect in GameStartEffects)
        {
            effect.Execute(new(this), new());
        }
    }

    public void ExecuteWhenPlacedEffects()
    {
        foreach (var effect in WhenPlacedEffects)
        {
            effect.Execute(new(this), new());
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
        return Owner.IsOpposingTo(player);
    }

    public bool IsFriendlyTo(Player player)
    {
        return !Owner.IsOpposingTo(player);
    }

    public bool IsOffBoard()
    {
        return GetStatus() == FighterStatus.OffBoard;
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
        if (Match.Map.GetFighterLocationOrDefault(this) is null)
        {
            return FighterStatus.OffBoard;
        }
        return FighterStatus.Alive;
    }

    public async Task<int> ProcessDamage(int amount, bool isCombatDamage = false)
    {
        if (amount <= 0) return 0;
        if (!IsAlive()) return 0;

        amount = Match.ModifyDamage(this, isCombatDamage, amount);

        var dealt = Health.DealDamage(amount);
        Match.Logger?.LogDebug("{FighterLogName} is dealt {Amount} damage (original amount: {OriginalAmount})", LogName, dealt, amount);
        Match.Logs.Public($"Fighter {FormattedLogName} is dealt {amount}{(isCombatDamage ? " combat" : "")} damage");

        if (!IsAlive())
        {
            Match.Logs.Public($"Fighter {FormattedLogName} dies!");
            Match.Logger?.LogDebug("Fighter {FighterLogName} dies", LogName);
            Match.ExecuteOnFighterDefeatEffects(this);
            await Match.Map.RemoveFighterFromBoard(this);
            Match.CheckForWinners();
        }
        await Match.UpdateClients();

        // TODO does this go before death or after death
        if (dealt > 0)
        {
            // OnDamage effects
            var effects = Match.GetEffectCollectionThatAccepts(new(this), f => f.OnDamageEffects);
            // TODO order effects
            foreach (var (source, effect) in effects)
            {
                effect.Execute(new(source), new(this));
            }
        }

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

    public async Task<int> FullyRecoverHealth()
    {
        var recovered = Health.RecoverFully();
        Match.Logger?.LogDebug("{FighterLogName} recovers to full health", LogName);
        Match.Logs.Public($"Fighter {FormattedLogName} recovers to full health");

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

    public int GetMeleeRange()
    {
        return Template.MeleeRange;
    }

    public IEnumerable<Fighter> GetReachableFighters()
    {
        var range = GetMeleeRange();
        List<Fighter> result = [.. Match.Map.GetReachableFighters(this, range)];

        if (Template.IsRanged)
        {
            result.AddRange(Match.Map.GetRangedReachableFighters(this));
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
        return node.IsInZone(zones);
    }

    public IEnumerable<EffectCollection> GetTurnPhaseEffects(TurnPhaseTrigger trigger)
    {
        if (TurnPhaseEffects.TryGetValue(trigger, out EffectCollection? value))
            return [value];
        return [];
    }

    public void SetName(string newName)
    {
        Name = newName;
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
            Key = Template.Key,
        };
    }

    public SetupData GetSetupData()
    {
        return new()
        {
            Id = Id,
            Key = Template.Key,
            Name = Name,
        };
    }

    public class Data
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Key { get; init; }
        public required bool IsAlive { get; init; }
        public required int CurHealth { get; init; }
        public required int MaxHealth { get; init; }
    }

    public class SetupData
    {
        public required int Id { get; init; }
        public required string Key { get; init; }
        public required string Name { get; init; }
    }
}

public class Health(Fighter fighter)
{
    public int Current { get; set; } = fighter.Template.StartingHealth;
    public int Max { get; set; } = fighter.Template.MaxHealth;
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

    public int RecoverFully()
    {
        var old = Current;
        Current = Max;
        return Current - old;
    }

    public void Set(int value)
    {
        Current = value;
    }
}

public enum FighterStatus
{
    Alive = 0,
    Dead = 1,
    OffBoard = 2,
}