function _Create()
    return UM:Card()
        :Effect(
            'Deal 1 damage to each opposing fighter.',
            UM.Effects:DealDamage(
                UM:Static(1),
                UM.S:Fighters()
                    :NotOwnedBy(UM:EffectOwner())
                    :Build()
            )
        )
        :Build()
end
