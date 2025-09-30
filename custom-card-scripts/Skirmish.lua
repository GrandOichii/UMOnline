-- TODO not tested
function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: If you won the combat, choose one of the fighters in the combat and move them up to 2 spaces',
            UM:If(
                UM.Conditional:CombatWonBy(
                    UM.Player:EffectOwner()
                ),
                UM.Effects:MoveFighters(
                    UM.S:Fighters()
                    :InCombat()
                    :Single()
                    :Build()
                )
            )
        )
    :Build()
end