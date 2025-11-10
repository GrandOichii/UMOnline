using NLua;
using UMCore.Matches.Players;
using UMCore.Matches.Tokens;
using UMCore.Utility;

namespace UMCore.Matches.Effects;

public class EffectCollection : IHasText
{
    public string Text { get; }
    public List<Effect> Effects { get; }
    public Effect? FighterPredicate { get; } = null;
    public Effect? PlayerPredicate { get; } = null;

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

        LuaFunction? fighterPred = table["fighterPred"] as LuaFunction;
        if (fighterPred is not null)
        {
            FighterPredicate = new(fighterPred);
        }

        LuaFunction? playerPred = table["playerPred"] as LuaFunction;
        if (playerPred is not null)
        {
            PlayerPredicate = new(playerPred);
        }
    }

    public bool AcceptsFighter(Fighter source, Fighter fighter)
    {
        return FighterPredicate is null || FighterPredicate.ExecuteFighterCheck(source, source.Owner, fighter);
    }

    public string GetText()
    {
        return Text;
    }


    private void Execute(LuaTable? args)
    {
        foreach (var effect in Effects)
        {
            effect.Execute(args);
        }
    }

    public void Execute(Fighter fighter)
    {
        Execute(MatchScripts.CreateArgs(fighter, fighter.Owner));
    }

    public void Execute(Fighter fighter, Player owner)
    {
        Execute(MatchScripts.CreateArgs(fighter, owner));
    }

    public void Execute(Fighter fighter, Player owner, PlacedToken token)
    {
        Execute(MatchScripts.CreateArgs(fighter, owner, token));
    }

    /// <summary>
    /// Executes the effect collection, arguments look like: (args, player)
    /// </summary>
    /// <param name="fighter">Effect originator</param>
    /// <param name="owner">Effect originator owner</param>
    /// <param name="player">Player subject</param>
    public void Execute(Fighter fighter, Player owner, Player player)
    {
        var args = MatchScripts.CreateArgs(fighter, owner);
        foreach (var effect in Effects)
        {
            effect.Execute(args, player);
        }
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
        return Execute(MatchScripts.CreateArgs(fighter, owner));
    }

    public bool ExecuteFighterCheck(Fighter fighter, Player owner, Fighter checkedFighter)
    {
        var result = Execute(MatchScripts.CreateArgs(fighter, owner), checkedFighter);
        return LuaUtility.GetReturnAsBool(result);
    }

    public int ExecuteReturnModified(Fighter fighter, Player owner, int value)
    {
        var result = Execute(MatchScripts.CreateArgs(fighter, owner), value);
        return LuaUtility.GetReturnAsInt(result);
    }
}