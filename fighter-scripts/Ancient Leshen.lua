
-- Add +3 to the value of the Leshen's attacks if it already attacked this turn.

function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            UM.Select:Fighters():Named('Ancient Leshen'):BuildPredicate(),
            UM.Mod.Cards:AttackCards(
                UM.Number:Static(3)
            ),
            UM.Conditions:FighterAttackedThisTurn(
                UM.Fighter:Named('Ancient Leshen')
            )
        )
        :Build()
end