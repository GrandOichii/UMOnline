function _Create()
    return UM:Card()
        :Effect(
            'Deal 1 damage to each fighter.',
            UM.Effects:DealDamage(
                UM.S:Fighters()
                :Build(),
                UM:Static(1)
            )
        )
        :Build()
end
