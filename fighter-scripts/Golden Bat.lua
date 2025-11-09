
-- If you haven't taken a Maneuver action this turn, add +2 to the value of Golden Bat's attacks.

function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            UM.Select:Fighters():Named('Golden Bat'):BuildPredicate(),
            UM.Mod.Cards:AttackCards(UM.Number:Static(2)),
            UM.Conditions:Not(
                UM.Conditions:PerformedActionThisTurn(
                    UM.Actions.MANOEUVRE
                )
            )
        )
    :Build()
end
