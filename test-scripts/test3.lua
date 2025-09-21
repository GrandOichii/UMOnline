function _Create()
    return UM:Card()
        :Effect(
            'Deal 1 damage to each opposing fighter.',
            UM.Effects:DealDamage(
                UM.S:Fighters()
                    :NotOwnedBy(UM:EffectOwner())
                    :Build(),
                UM:Static(1)
            )
        )
        :Build()
end
