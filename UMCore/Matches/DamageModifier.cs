using NLua;
using UMCore.Utility;

namespace UMCore.Matches;

public class DamageModifier(Fighter fighter, LuaFunction func)
{
    public int Modify(Fighter damageTo, bool isCombatDamage, int amount)
    {
        var returned = func.Call(MatchScripts.CreateArgs(fighter, fighter.Owner), damageTo, isCombatDamage, amount);
        var result = LuaUtility.GetReturnAsInt(returned);
        if (result < 0) result = 0;
        return result;
    }
}