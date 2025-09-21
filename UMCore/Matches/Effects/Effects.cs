using NLua;

namespace UMCore.Matches.Effects;

public class EffectCollection
{
    public List<Effect> Effects { get; }

    public EffectCollection()
    {
        Effects = [];
    }

    public EffectCollection(LuaTable table)
    {
        // TODO
        Effects = [];

        foreach (var e in table.Values)
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