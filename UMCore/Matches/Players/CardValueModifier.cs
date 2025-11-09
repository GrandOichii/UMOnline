using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Effects;
using UMCore.Utility;

namespace UMCore.Matches.Players;

public class CardValueModifier(Fighter _fighter, Effect _effect)
{
    public int Modify(CombatPart card)
    {
        var returned = _effect.Execute(
            MatchScripts.CreateArgs(_fighter, _fighter.Owner),
            card,
            card.Value
        );
        return LuaUtility.GetReturnAsInt(returned);
    }
}