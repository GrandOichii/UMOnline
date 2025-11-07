

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may discard 1 card to gain 1 action.',
            UM.Effects:If(
                UM.Conditions:Gt(
                    UM.Count:CardsInHand(UM.Player:EffectOwner()),
                    UM.Number:Static(0)
                ),
                UM.Effects:Optional(
                    'Discard a card to gain 1 action?',
                    UM.Effects:Discard(
                        UM.Select:Players():You():Build(),
                        1, false
                    ),
                    UM.Effects:GainActions(1)
                )
            )
        )
    :Build()
end