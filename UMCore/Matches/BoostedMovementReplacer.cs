
using NLua;
using UMCore.Utility;

namespace UMCore.Matches;

public class BoostedMovementReplacer(Fighter fighter, LuaFunction func)
{
    public bool TryReplace(Movement movement)
    {
        var returned = func.Call(MatchScripts.CreateArgs(fighter, fighter.Owner), movement.Fighter);
        var result = LuaUtility.GetReturnAsBool(returned);
        return result;
    }
}