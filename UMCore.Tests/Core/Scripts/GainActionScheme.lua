
function _Create()
    return UM.Build:Card()
        :Effect(
            'Gain 1 action.',
            UM.Effects:GainActions(1)
        )
        :Build()
end