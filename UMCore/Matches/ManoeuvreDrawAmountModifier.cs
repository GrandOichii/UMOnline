using NLua;
using UMCore.Matches.Players;
using UMCore.Utility;

namespace UMCore.Matches;

public class ManoeuvreDrawAmountModifier(Fighter fighter, LuaFunction func)
{
    public int Modify(Player player, int amount)
    {
        var returned = func.Call(MatchScripts.CreateArgs(fighter, fighter.Owner), player, amount);
        var result = LuaUtility.GetReturnAsInt(returned);
        if (result < 0) result = 0;
        return result;
    }
}