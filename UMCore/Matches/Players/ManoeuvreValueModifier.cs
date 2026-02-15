using NLua;
using UMCore.Matches.Effects;

namespace UMCore.Matches.Players;

public class ManoeuvreValueModifier
{
    private readonly Fighter _fighter;
    private readonly Effect _fighterPredicate;
    private readonly Effect _modFunc;

    public ManoeuvreValueModifier(Fighter fighter, LuaTable table)
    {
        _fighter = fighter;
        _fighterPredicate = new((table["fighterPred"] as LuaFunction)
            ?? throw new MatchException($"Failed to get fighter predicate for ManoeuvreValueModifier"));
        _modFunc = new((table["modFunc"] as LuaFunction)
            ?? throw new MatchException($"Failed to get modification function for ManoeuvreValueModifier"));
    }

    public bool Accepts(Fighter fighter)
    {
        return _fighterPredicate.ExecuteCheck(new(_fighter), new(fighter));
    }

    public int Modify(int original)
    {
        return _modFunc.Modify(new(_fighter), new(), original);
    }
}