function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: Deal 1 damage to each opposing fighter.',
            UM.Effects:DealDamage(
                UM:Static(1),
                UM.S:Fighters()
                :OpposingTo(UM.Players:EffectOwner())
                :Build()
            )
        )
        :Build()
end
