function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may deal 1 damage to a fighter adjacent to Dracula. If you do, draw a card.',
            UM:If(
                UM.Conditional:FightersCountGte(
                    UM.S:Fighters():AdjacentTo(UM.Fighters:Source()):Build(),
                    UM:Static(1)
                ),
                UM.Effects:Optional(
                    'Deal 1 damage to an adjacent fighter and draw a card?',
                    UM.Effects:DealDamage(
                        UM:Static(1),
                        UM.S:Fighters():AdjacentTo(UM.Fighters:Source()):Build()
                    ),
                    UM.Effects:Draw(
                        UM:Static(1)
                    )
                )
            )
        )
    :Build()
end
