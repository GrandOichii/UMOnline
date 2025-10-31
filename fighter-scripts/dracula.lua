function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may deal 1 damage to a fighter adjacent to Dracula. If you do, draw a card.',
            UM:If(
                UM.Conditions:CountGte(
                    UM.Count:Fighters(
                        UM.Select:Fighters()
                            :AdjacentTo(UM.Fighter:Source())
                            :Build()
                    ),
                    UM:Static(1)
                ),
                UM.Effects:Optional(
                    'Deal 1 damage to an adjacent fighter and draw a card?',
                    UM.Effects:DealDamage(
                        UM.S:Fighters():AdjacentTo(UM.Fighters:Source()):Build(),
                        UM:Static(1)
                    ),
                    UM.Effects:Draw(
                        UM.Select:Players():You():Build(),
                        UM:Static(1),
                        false
                    )
                )
            )
        )
    :Build()
end
