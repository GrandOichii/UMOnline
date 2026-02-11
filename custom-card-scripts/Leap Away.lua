function _Create()
    return UM.Build:Card()
        :AfterCombat(
            'After combat: If you won the combat, choose one of the fighters in combat and move them up to 4 spaces',
            UM.Effects:If(
                UM.Conditions:CombatWonBy(
                    UM.Player:EffectOwner()
                ),
                UM.Effects:MoveFighters(
                    UM.Select:Fighters()
                        :InCombat()
                        :Single()
                        :Build(),
                    UM.Number:UpTo(4)
                )
            )
        )
    :Build()
end