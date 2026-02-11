function _Create()
    return UM.Build:Card()
        :AfterCombat(
            'After combat: If King Arthur has 4 or less health but is not defeated, set his health to 8.',
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions:Lte(
                        UM.Count:FighterHealth(
                            UM.Fighter:Named('King Arthur')
                        ),
                        UM.Number:Static(4)
                    ),
                    UM.Conditions:FighterIsUndefeated(
                        UM.Fighter:Named('King Arthur')
                    )
                ),
                UM.Effects:SetHealth(
                    UM.Select:Fighters()
                        :Named('King Arthur')
                        :Build(),
                    4
                )
            )
        )
    :Build()
end