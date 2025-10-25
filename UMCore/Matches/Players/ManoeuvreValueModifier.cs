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
        _fighterPredicate = new((table["fighterPred"] as LuaFunction)!); // TODO check for null
        _modFunc = new((table["modFunc"] as LuaFunction)!); // TODO check for null
    }

    public bool Accepts(Fighter fighter)
    {
        return _fighterPredicate.ExecuteFighterCheck(_fighter, _fighter.Owner, fighter);
    }

    public int Modify(int original)
    {
        return _modFunc.ExecuteReturnModified(_fighter, _fighter.Owner, original);
    }
}