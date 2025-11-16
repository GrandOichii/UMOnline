
function _Create()
    return UM.Build:Fighter()
        :OnBoost(
            'Whenever you BOOST, draw 1 card.',
            UM.Select:Players():You():BuildPredicate(),
            UM.Effects:Draw(
                UM.Select:Players():You():Build(),
                UM.Number:Static(1), false
            )
        )
        :DefineCardZone(
            'VIBRANIUM SUIT',
            UM.Build:CardZone()
                :Build()
        )
        :AddBoostSource(
            'Cards stored in your VIBRANIUM SUIT can only be used to BOOST.',
            'VIBRANIUM SUIT'
        )
        -- TODO define vibranium suit custom card zone
    :Build()
end