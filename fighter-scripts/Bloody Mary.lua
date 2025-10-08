function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, if you have exactly 3 cards in hand, gain 1 action.',
            UM.Effects:If(
                UM.Conditions:Eq(
                    UM.Count:CardsInHand(
                        UM.Player:EffectOwner()
                    ),
                    UM.Number:Static(3)
                ),
                UM.Effects:GainActions(1)
            )
        )
    :Build()
end
