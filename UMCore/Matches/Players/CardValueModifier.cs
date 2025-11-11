using NLua;
using UMCore.Matches.Attacks;
using UMCore.Matches.Effects;
using UMCore.Utility;

namespace UMCore.Matches.Players;

public class CardValueModifier(Fighter _fighter, EffectCollection _effect)
{
    public int Modify(CombatPart card)
    {
        return _effect.Modify(
            new(_fighter),
            new(card),
            card.Value
        );
    }
}