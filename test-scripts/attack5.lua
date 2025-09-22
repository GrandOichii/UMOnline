function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: If you won the combat, deal 8 damage to the opposing fighter.',
            UM:If(
                UM.Conditional:CombatWonBy(
                    UM.Players:EffectOwner()
                ),
                UM.Effects:DealDamage(
                    UM:Static(8),
                    UM.S:Fighters()
                    :OpposingInCombatTo(UM.Fighters:Source())
                    :Single()
                    :Build()
                )
            )
        )
        :Build()
end
