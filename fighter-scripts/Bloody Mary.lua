function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, if you have exactly 3 cards in hand, gain 1 action.',
            UM:If(
                UM.Conditional:Eq(
                    UM.Count:CardsInHand(
                        UM.Players:EffectOwner()
                    ),
                    UM:Static(3)
                ),
                UM.Effects:GainActions(
                    UM:Static(1)
                )
            )
        )
    :Build()
end
