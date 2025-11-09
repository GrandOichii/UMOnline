
function _Create()
    return UM.Build:Fighter()
        :AfterScheme(
            'After Triss plays a scheme, deal 2 damage to a fighter adjacent to Triss.',
            UM.Select:Fighters():Named('Triss'):BuildPredicate(),
            UM.Effects:DealDamage(
                UM.Select:Fighters()
                    :AdjacentTo(UM.Fighter:Named('Triss'))
                    :Single()
                    :Build(),
                UM.Number:Static(2)
            )
        )
    :Build()
end