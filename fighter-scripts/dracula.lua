function _Create()
    local fighterSelector = UM.Select:Fighters()
        :AdjacentTo(UM.Fighter:Source())
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may deal 1 damage to a fighter adjacent to Dracula. If you do, draw a card.',
            UM.Effects:If(
                UM.Conditions:CountGte(
                    fighterSelector:Build(), 1
                ),
                UM.Effects:Optional(
                    'Deal 1 damage to an adjacent fighter and draw a card?',
                    UM.Effects:DealDamage(
                        fighterSelector:Single():Build(),
                        UM.Number:Static(1)
                    ),
                    UM.Effects:Draw(
                        UM.Select:Players():You():Build(),
                        UM.Number:Static(1),
                        false
                    )
                )
            )
        )
    :Build()
end
