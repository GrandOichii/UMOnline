function _Create()
    return UM:Card()
        :Effect(
            'Draw 2 cards. Gain 1 action.',
            UM.Effects:Draw(
                UM:Static(2)
            ),
            UM.Effects:GainActions(
                UM:Static(1)
            )
        )
        :Build()
end
