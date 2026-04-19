
function _Create()
    return UM.Build:Card()
        :Effect(
            'Draw 1 card.',
            UM.Effects:Draw(
                UM.Select:Players()
                    :You()
                    :Build(),
                UM.Number:Static(1)
            )
        )
        :Build()
end