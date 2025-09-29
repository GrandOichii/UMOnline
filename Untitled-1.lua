--

function _Create()
    return UM:Card()
        :AfterCombat(
            'Choose an empty space in this fighter\'s zone. Place this fighter in that space.',
            UM.Effects:Choose(
                UM.S.Spaces()
                :Empty()
                :BuildOne()
            ),
            UM.Effects:PlaceFighter(
                UM.S:Fighters()
                :Only(UM.Effects:Source())
                :Build(),
                UM.S.Spaces()
                :Only(UM.Ctx:GetLastSpace())
                :BuildOne()
            )
        )
        :Build()
end

-- After combat: If Holmes is adjacent to the opposing fighter, deal 2 damage to that fighter.

function _Create()
    return UM:Card()
        :AfterCombat(
            UM.Effects:Conditional(
                UM:If(
                    UM.Conditional:IsAdjacentTo(
                        
                    )
                )
            )
        )
    :Build()
end