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

    public string FormattedLogName => $"[{Id}:{GetName()}]";

    public List<CardValueModifier> CardValueModifiers { get; }
    public List<EffectCollection> WhenPlacedEffects { get; }
    public List<ManoeuvreValueModifier> ManoeuvreValueMods { get; }
    public List<FighterPredicateEffect> OnAttackEffects { get; }
    public List<FighterPredicateEffect> AfterAttackEffects { get; }
    public List<EffectCollection> GameStartEffects { get; }
    public List<MovementNodeConnection> MovementNodeConnections { get; }
    public List<CardCancellingForbid> CardCancellingForbids { get; }
    public List<EffectCollection> OnManoeuvreEffects { get; }
    public List<EffectCollection> OnDamageEffects { get; }
    public List<FighterPredicateEffect> OnFighterDefeatEffects { get; }
    public CombatStepEffectsCollection CombatStepEffects { get; }
    public List<DamageModifier> DamageModifiers { get; }
    public List<FighterPredicateEffect> AfterMovementEffects { get; }
    public List<BoostedMovementReplacer> BoostedMovementReplacers { get; }
    public List<OnMoveEffect> OnMoveEffects { get; }
    public List<ManoeuvreDrawAmountModifier> ManoeuvreDrawAmountModifiers { get; }

    public static List<FighterPredicateEffect> ExtractFighterPredicateEffects(Fighter fighter, LuaTable data, string key)
    {
        try
        {
            List<FighterPredicateEffect> result = [];
            var table = LuaUtility.TableGet<LuaTable>(data, key);
            foreach (var value in table.Values)
            {
                var tableRaw = value as LuaTable;
                // TODO check for null
                result.Add(new(fighter, tableRaw!));
            }
            return result;
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed extract {key} predicate effects for fighter {fighter.Name}", e);
        }
    }

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

        CardValueModifiers = [];
        try
        {
            var cardValueModifiers = LuaUtility.TableGet<LuaTable>(data, "CardValueModifiers");
            foreach (var keyRaw in cardValueModifiers.Keys)
            {
                var modFunc = cardValueModifiers[keyRaw] as LuaFunction;
                // TODO check for null
                CardValueModifiers.Add(new(this, new(modFunc!)));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get card value modifiers for fighter {template.Name}", e);
        }

        WhenPlacedEffects = [];
        try
        {
            var whenPlacedEffects = LuaUtility.TableGet<LuaTable>(data, "WhenPlacedEffects");
            foreach (var value in whenPlacedEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                var effects = new EffectCollection(table!);
                WhenPlacedEffects.Add(effects);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get initial fighter placement effects for fighter {template.Name}", e);
        }

        ManoeuvreValueMods = [];
        try
        {
            var manoeuvreValueMods = LuaUtility.TableGet<LuaTable>(data, "ManoeuvreValueMods");
            foreach (var value in manoeuvreValueMods.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                ManoeuvreValueMods.Add(new(this, table!));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get manoeuvre value modifiers for fighter {template.Name}", e);
        }

        OnAttackEffects = [];
        try
        {
            var onAttackEffects = LuaUtility.TableGet<LuaTable>(data, "OnAttackEffects");
            foreach (var value in onAttackEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                OnAttackEffects.Add(new(this, table!));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get on attack effects for fighter {template.Name}", e);
        }

        AfterAttackEffects = [];
        try
        {
            var onAttackEffects = LuaUtility.TableGet<LuaTable>(data, "AfterAttackEffects");
            foreach (var value in onAttackEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                AfterAttackEffects.Add(new(this, table!));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get after attack effects for fighter {template.Name}", e);
        }

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

        GameStartEffects = [];
        try
        {
            var gameStartEffects = LuaUtility.TableGet<LuaTable>(data, "GameStartEffects");
            foreach (var value in gameStartEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                var effects = new EffectCollection(table!);
                GameStartEffects.Add(effects);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get game start effects for fighter {template.Name}", e);
        }

        MovementNodeConnections = [];
        try
        {
            var movementNodeConnections = LuaUtility.TableGet<LuaTable>(data, "MovementNodeConnections");
            foreach (var value in movementNodeConnections.Values)
            {
                var func = value as LuaFunction;
                // TODO check for null
                var effects = new MovementNodeConnection(this, func!);
                MovementNodeConnections.Add(effects);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to movement node connections for fighter {template.Name}", e);
        }

        CardCancellingForbids = [];
        try
        {
            var cardCancellingForbids = LuaUtility.TableGet<LuaTable>(data, "CardCancellingForbids");
            foreach (var value in cardCancellingForbids.Values)
            {
                var func = value as LuaFunction;
                // TODO check for null
                var effects = new CardCancellingForbid(this, func!);
                CardCancellingForbids.Add(effects);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to movement node connections for fighter {template.Name}", e);
        }

        OnManoeuvreEffects = [];
        try
        {
            var onManoeuvreEffects = LuaUtility.TableGet<LuaTable>(data, "OnManoeuvreEffects");
            foreach (var value in onManoeuvreEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                var effects = new EffectCollection(table!);
                OnManoeuvreEffects.Add(effects);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get on manoeuvre effects for fighter {template.Name}", e);
        }

        OnDamageEffects = [];
        try
        {
            var onDamageEffects = LuaUtility.TableGet<LuaTable>(data, "OnDamageEffects");
            foreach (var value in onDamageEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                var effects = new EffectCollection(table!);
                OnDamageEffects.Add(effects);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get on damage effects for fighter {template.Name}", e);
        }

        OnFighterDefeatEffects = [];
        try
        {
            var onDefeatEffects = LuaUtility.TableGet<LuaTable>(data, "OnFighterDefeatEffects");
            foreach (var value in onDefeatEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                OnFighterDefeatEffects.Add(new(this, table!));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get on fighter defeat effects for fighter {template.Name}", e);
        }

        // combat step effects
        try
        {
            CombatStepEffects = new(data);
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get combat step effects for fighter {Name}", e);
        }
        
        DamageModifiers = [];
        try
        {
            var damageModifiers = LuaUtility.TableGet<LuaTable>(data, "DamageModifiers");
            foreach (var value in damageModifiers.Values)
            {
                var func = value as LuaFunction;
                // TODO check for null
                var effects = new DamageModifier(this, func!);
                DamageModifiers.Add(effects);
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to damage modifiers for fighter {template.Name}", e);
        }
    
        AfterMovementEffects = ExtractFighterPredicateEffects(this, data, "AfterMovementEffects");

        BoostedMovementReplacers = [ ..ExtractFunctionList(this, data, "BoostedMovementReplacers")
            .Select(f =>
                new BoostedMovementReplacer(this, f)
            )
        ];

        OnMoveEffects = [];
        try
        {
            var onMoveEffects = LuaUtility.TableGet<LuaTable>(data, "OnMoveEffects");
            foreach (var value in onMoveEffects.Values)
            {
                var table = value as LuaTable;
                // TODO check for null
                OnMoveEffects.Add(new(this, table!));
            }
        }
        catch (Exception e)
        {
            throw new MatchException($"Failed to get on fighter defeat effects for fighter {template.Name}", e);
        }

        ManoeuvreDrawAmountModifiers = [ ..ExtractFunctionList(this, data, "ManoeuvreDrawAmountModifiers")
            .Select(f =>
                new ManoeuvreDrawAmountModifier(this, f)
            )
        ];
    }
    
    public void ExecuteGameStartEffects()
    {
        foreach (var effect in GameStartEffects)
        {
            effect.Execute(this, Owner);
        }
    }

    public void ExecuteWhenPlacedEffects()
    {
        foreach (var effect in WhenPlacedEffects)
        {
            effect.Execute(this, Owner);
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
            Match.CheckForWinners();
            await Match.Map.RemoveFighterFromBoard(this);
            Match.ExecuteOnFighterDefeatEffects(this);
        }
        await Match.UpdateClients();

        // TODO does this go before death or after death
        if (dealt > 0)
        {
            await ExecuteOnDamageEffects();            
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

    public async Task ExecuteOnManoeuvreEffects()
    {
        List<EffectCollection> effects = [.. OnManoeuvreEffects];
        // TODO order effects
        foreach (var effect in effects)
            effect.Execute(this, Owner);
    }

    public async Task ExecuteOnDamageEffects()
    {
        List<EffectCollection> effects = [.. OnDamageEffects];
        // TODO order effects
        foreach (var effect in effects)
            effect.Execute(this, Owner);
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