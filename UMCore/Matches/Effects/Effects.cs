using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Utility;

namespace UMCore.Matches.Effects;

public class EffectCollectionArgs
{
    public Fighter Fighter { get; }
    public PlacedToken? Token { get; }
    public LuaTable Ctx { get; }

    public EffectCollectionArgs(Fighter fighter, PlacedToken? token)
    {
        Fighter = fighter;
        Token = token;
        Ctx = LuaUtility.CreateTable(fighter.Match.LState);
    }

    public EffectCollectionArgs(Fighter fighter) : this(fighter, null) { }

    public LuaTable ToLuaTable(Match match)
    {
        return LuaUtility.CreateTable(match.LState, new Dictionary<string, object?>()
        {
            { "fighter", Fighter },
            { "owner", Fighter.Owner },
            { "token", Token },
            { "ctx", Ctx },
        });
    }
}

public class EffectCollectionSubjects
{
    public Fighter? Fighter { get; }
    public Player? Player { get; }
    public CombatPart? CombatPart { get; }

    public EffectCollectionSubjects(Fighter? fighter, Player? player, CombatPart? part)
    {
        Fighter = fighter;
        Player = player;
        CombatPart = part;
    }

    public EffectCollectionSubjects() : this(null, null, null) { }
    public EffectCollectionSubjects(Fighter fighter) : this(fighter, null, null) { }
    public EffectCollectionSubjects(Player player) : this(null, player, null) { }
    public EffectCollectionSubjects(CombatPart part) : this(null, null, part) { }

    public LuaTable ToLuaTable(Match match)
    {
        return LuaUtility.CreateTable(match.LState, new Dictionary<string, object?>()
        {
            { "fighter", Fighter },
            { "player", Player },
            { "combatPart", CombatPart },
        });
    }
}

public class EffectCollection : IHasText
{
    public string Text { get; }
    public List<Effect> Effects { get; }
    // public Effect? FighterPredicate { get; } = null;
    // public Effect? PlayerPredicate { get; } = null;
    public Effect Condition { get; }

    public EffectCollection(LuaTable table)
    {
        Effects = [];

        Text = LuaUtility.TableGet<string>(table, "text");

        var effectsRaw = LuaUtility.TableGet<LuaTable>(table, "effects");
        foreach (var e in effectsRaw.Values)
        {
            var effectFunc = e as LuaFunction
                ?? throw new MatchException($"Incorrect table format for constructing {nameof(EffectCollection)}");

            Effects.Add(new(effectFunc!));
        }

        Condition = new((table["cond"] as LuaFunction)!);
    }

    public bool ConditionsMet(EffectCollectionArgs args, EffectCollectionSubjects subjects)
    {
        return Condition.ExecuteCheck(args, subjects);
    }

    public string GetText()
    {
        return Text;
    }

    public void Execute(EffectCollectionArgs args, EffectCollectionSubjects subjects)
    {
        if (!ConditionsMet(args, subjects)) return;

        foreach (var effect in Effects)
        {
            effect.Execute(args, subjects);
        }
    }

    public int Modify(EffectCollectionArgs args, EffectCollectionSubjects subjects, int value)
    {
        var subs = subjects ?? new();
        if (!ConditionsMet(args, subs)) return value;

        var result = value;
        foreach (var effect in Effects)
        {
            result = effect.Modify(args, subs, result);
        }
        return result;
    }
}

public class Effect
{
    public LuaFunction Func { get; }

    public Effect(LuaFunction func)
    {
        Func = func;
    }

    public bool ExecuteCheck(EffectCollectionArgs args, EffectCollectionSubjects subjects)
    {
        var match = args.Fighter.Match;
        var returned = Func.Call(
            args.ToLuaTable(match),
            subjects.ToLuaTable(match)
        );
        return LuaUtility.GetReturnAsBool(returned);
    }

    public void Execute(EffectCollectionArgs args, EffectCollectionSubjects subjects)
    {
        var match = args.Fighter.Match;
        Func.Call(
            args.ToLuaTable(match),
            subjects.ToLuaTable(match)
        );
    }
    
    public int Modify(EffectCollectionArgs args, EffectCollectionSubjects subjects, int value)
    {
        var match = args.Fighter.Match;
        var returned = Func.Call(
            args.ToLuaTable(match),
            subjects.ToLuaTable(match),
            value
        );

        return LuaUtility.GetReturnAsInt(returned);
    }
}