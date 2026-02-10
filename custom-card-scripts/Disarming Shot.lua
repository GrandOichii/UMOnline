function _Create()
    return UM.Build:Card()
        :AfterCombat(
            'After combat: Draw a number of cards equal to the amount of damage dealt to the opposing fighter',
            UM.Effects:Draw(
                UM.Select:Players()
                    :You()
                    :Build(),
                UM.Count:DealtCombatDamage()
            )
        )
    :Build()
end