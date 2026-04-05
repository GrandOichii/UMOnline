using NLua;
using UMCore.Matches.Players.Cards;
using UMCore.Utility;

namespace UMCore.Matches.Effects;

public class CardZoneChangeRedirector(Fighter fighter, LuaFunction function)
{
    public Fighter Fighter { get; } = fighter;

    public bool Redirect(CardZoneChange zoneChange)
    {
        var args = MatchScripts.CreateArgs(Fighter, Fighter.Owner);

        var returned = function.Call(args, zoneChange);

        return LuaUtility.GetReturnAsBool(returned);
    }
}