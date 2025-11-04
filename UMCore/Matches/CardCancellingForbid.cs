using NLua;
using UMCore.Matches.Cards;
using UMCore.Utility;

namespace UMCore.Matches;

public class CardCancellingForbid(Fighter owner, LuaFunction func)
{
    public bool Forbids(MatchCard card)
    {
        var returned = func.Call(LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object>()
        {
            { "fighter", owner },
            { "owner", owner.Owner },
        }), card, card.Owner);

        return LuaUtility.GetReturnAsBool(returned);
    }

}