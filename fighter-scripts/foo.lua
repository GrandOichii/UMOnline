function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may draw a card.',
            UM.Effects:Optional(
                'Draw a card?',
                UM.Effects:Draw(
                    UM:Static(1)
                )
        )
        )
    :Build()
end
