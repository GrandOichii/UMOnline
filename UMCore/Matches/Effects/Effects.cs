using NLua;
using UMCore.Utility;

namespace UMCore.Matches.Effects;

public class EffectCollection
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
            var effectFunc = e as LuaFunction;
            // TODO throw exception if null

            Effects.Add(new(effectFunc!));
        }
    }

    public void Execute(LuaTable? args = null)
    {
        foreach (var effect in Effects)
            effect.Execute(args);
    }
}

public class Effect
{
    public LuaFunction Func { get; }

    public Effect(LuaFunction func)
    {
        Func = func;
    }

    public void Execute(LuaTable? args = null)
    {
        Func.Call(args);
    }
}