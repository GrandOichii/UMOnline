using NLua;
using UMCore.Utility;

namespace UMCore.Matches.Cards;

public class SchemeRequirement(LuaTable data) : IHasText
{
    private readonly string _text = LuaUtility.TableGet<string>(data, "text");
    private readonly LuaFunction _checkFunc = LuaUtility.TableGet<LuaFunction>(data, "checkFunc");

    public string GetText() => _text;

    public bool ConditionsMet(Fighter fighter)
    {
        var returned = _checkFunc.Call(MatchScripts.CreateArgs(fighter, fighter.Owner));
        return LuaUtility.GetReturnAsBool(returned);
    }
}