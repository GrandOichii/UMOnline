-- After combat: choose an empty space in this fighter's zone. Place this fighter in that space.

function _Create()
    return UM.Build:Card()
        :AfterCombat(
            'After combat: Choose an empty space in this fighter\'s zone. Place this fighter in that space.',
            UM.Effects:PlaceFighter(
                UM.Fighter:Source(),
                UM.Select:Nodes()
                    :InZoneOfFighter(
                        UM.Fighter:Source()
                    )
                    :Empty()
                    :BuildOne()
            )
        )
        :Build()
end
