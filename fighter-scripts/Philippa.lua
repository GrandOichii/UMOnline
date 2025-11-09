
-- 

function _Create()
    return UM.Build:Fighter()
        :AtTheEndOfYourTurn(
            'At the end of your turn, you may draw until you have a hand of 4 cards.',
            UM.Effects:If(
                UM.Conditions:Lt(
                    UM.Count:CardsInHand(
                        UM.Player:EffectOwner()
                    ),
                    UM.Number:Static(4)
                ),
                UM.Effects:Optional(
                    'Draw until you have a hand of 4 cards?',
                    UM.Effects:DrawTo(
                        UM.Select:Players():You():Build(),
                        UM.Number:Static(4)
                    )
                )
            )
        )
    :Build()
end