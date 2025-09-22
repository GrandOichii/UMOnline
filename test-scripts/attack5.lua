function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: Place this fighter in any space.',
            UM.Effects:PlaceFighter(
                UM.S:Fighters()
                :Only(UM.Fighters:Source())
                :Single()
                :Build(),
                UM.S:Spaces()
                --
                :Build()
            )
        )
        :Build()
end
