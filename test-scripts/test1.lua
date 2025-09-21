function _Create()
    return UM:Card()
        :Effect(
            'Draw 3 cards.',
            UM.Effects:Draw(
                UM:Static(3)
            )
        )
        :Build()
end
