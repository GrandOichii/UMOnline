function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the end of your turn, if there are no opposing fighters in Bigfoot\'s zone, you may draw 1 card.',
            UM:If(
                UM.Conditional:Eq(
                    UM.S:Fighters()
                        :OpposingTo(UM.Players:EffectOwner())
                        :InSameZoneAs(UM.Fighters:Source())
                        :Build(),
                    UM:Static(0)
                ),
                UM.Effects:Optional(
                    'Draw a card?',
                    UM.Effects:Draw(
                        UM:Static(1)
                    )
                )
            )
        )
    :Build()
end
