function _Create()
    return UM.Build:Card()
        :AfterCombat(
            'After combat: If you won the combat, look at your opponent\'s hand and choose 1 card for them to discard.',
            UM.Effects:If(
                UM.Conditions:CombatWonBy(
                    UM.Player:EffectOwner()
                ),
                UM.Effects:LookAtHandAndForceToDiscard(
                    UM.Player:Opponent(),
                    UM.Number:Static(1)
                )
            )
        )
    :Build()
end