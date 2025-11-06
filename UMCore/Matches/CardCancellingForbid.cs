using NLua;
using UMCore.Matches.Cards;
using UMCore.Matches.Players;
using UMCore.Utility;

namespace UMCore.Matches;

public class CardCancellingForbid(Fighter owner, LuaFunction func)
{
    public bool Forbids(MatchCard card, Player cancelBy)
    {
        var returned = func.Call(MatchScripts.CreateArgs(owner, owner.Owner), card, cancelBy);

        return LuaUtility.GetReturnAsBool(returned);
    }

}