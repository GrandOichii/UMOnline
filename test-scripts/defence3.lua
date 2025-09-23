function _Create()
    return UM:Card()
        :Immediately(
            'Immediately: Cancel all effects on your opponent\'s card.',
            UM.Effects:CancelAllEffectsOfOpponentsCard()
        )
        :Build()
end
