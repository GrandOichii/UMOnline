
function _Create()
    return UM.Build:Fighter()
        :DuringCombat(
            'During combat: If you have 2 or fewer cards in your hand, you may BLIND BOOST your attack or defense.',
            UM.Effects:If(
                UM.Conditions:Lte(
                    UM.Count:CardsInHand(
                        UM.Player:EffectOwner()
                    ),
                    UM.Number:Static(2)
                ),
                UM.Effects:Optional(
                    'BLIND BOOST your card?',
                    UM.Effects:BlindBoost(
                        UM.Number:Static(1),
                        UM.Select:Players():You():Build()
                    )
                )
            )
        )
    :Build()
end