using NLua;
using UMCore.Matches.Players.Cards;
using UMCore.Utility;

namespace UMCore.Matches;

public class CardZoneChangeRedirector(Fighter fighter, LuaFunction function)
{
    public bool Redirect(CardZoneChange zoneChange)
    {
        var args = MatchScripts.CreateArgs(fighter, fighter.Owner);

        var returned = function.Call(args, zoneChange);

        return LuaUtility.GetReturnAsBool(returned);
    }
}