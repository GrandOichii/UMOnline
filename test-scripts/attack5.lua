function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: Place Foo in any space in her zone.',
            UM.Effects:PlaceFighter(
                UM.S:Fighters()
                :Named('Foo')
                :Single()
                :Build(),
                UM.S:Spaces()
                :InSameZoneAs(UM.Fighters:Source())
                :Build()
            )
        )
        :Build()
end
