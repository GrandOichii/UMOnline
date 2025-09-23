function _Create()
    return UM:Card()
        :Effect(
            'Deal 2 damage to any one fighter in Medusa\'s zone.',
            UM.Effects:DealDamage(
                UM:Static(2),
                UM.S:Fighters()
                :InSameZoneAs(UM.S:Fighters():Named('Foo'):BuildOne())
                :Single()
                :Build()
            )
        )
        :Build()
end
