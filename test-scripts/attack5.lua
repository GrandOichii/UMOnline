function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: Deal 1 damage to Foo.',
            UM.Effects:DealDamage(
                UM:Static(1),
                UM.S:Fighters()
                :Named('Foo')
                :Single()
                :Build()
            )
        )
        :Build()
end
