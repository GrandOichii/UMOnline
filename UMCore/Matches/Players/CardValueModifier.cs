using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Effects;
using UMCore.Utility;

namespace UMCore.Matches.Players;

public class CardValueModifier(Effect effect)
{
    public int Modify(CombatPart card)
    {
        var owner = card.Card.Owner;
        var returned = effect.Execute(
            LuaUtility.CreateTable(owner.Match.LState, new Dictionary<string, object>()
            {
                { "fighter", card.Fighter },
                { "owner", owner },
            }),
            card,
            card.Value
        );
        return LuaUtility.GetReturnAsInt(returned);
    }
}