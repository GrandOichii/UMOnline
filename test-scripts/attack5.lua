function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: If you won the combat, your opponent discards 1 card.',
            UM:If(
                UM.Conditional:CombatWonBy(
                    UM.Players:EffectOwner()
                ),
                UM.Effects:Discard(
                    UM.Players:Opponent(),
                    UM:Static(1),
                    false
                )
            )
        )
        :Build()
end
