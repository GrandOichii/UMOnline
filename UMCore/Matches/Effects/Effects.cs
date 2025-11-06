using NLua;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Utility;

namespace UMCore.Matches.Effects;

public class EffectCollection : IHasText
{
    public string Text { get; }
    public List<Effect> Effects { get; }

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
    }

    public string GetText()
    {
        return Text;
    }


    public void Execute(LuaTable? args = null)
    {
        foreach (var effect in Effects)
        {
            effect.Execute(args);
        }
    }

    public void Execute(Fighter fighter, Player owner)
    {
        Execute(LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", fighter },
            { "owner", owner },
        }));
    }

    public void Execute(Fighter fighter, Player owner, PlacedToken token)
    {
        Execute(LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", fighter },
            { "owner", owner },
            { "token", token },
        }));
    }
}

public class Effect
{
    public LuaFunction Func { get; }

    public Effect(LuaFunction func)
    {
        Func = func;
    }

    public object[] Execute(params object?[] args)
    {
        return Func.Call(args);
    }

    public object[] Execute(Fighter fighter, Player owner)
    {
        return Execute(LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", fighter },
            { "owner", owner },
        }));
    }

    public bool ExecuteFighterCheck(Fighter fighter, Player owner, Fighter checkedFighter)
    {
        var result = Execute(LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", fighter },
            { "owner", owner },
        }), checkedFighter);
        return LuaUtility.GetReturnAsBool(result);
    }

    public int ExecuteReturnModified(Fighter fighter, Player owner, int value)
    {
        var result = Execute(LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", fighter },
            { "owner", owner },
        }), value);
        return LuaUtility.GetReturnAsInt(result);
    }
}