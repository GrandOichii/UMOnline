function _Create()
    return UM.Build:Card()
        :DuringCombat(
            'During combat: Discard the top card. Add its BOOST value to this card\'s value',
            UM.Effects:Discard(
                UM.Select:Players()
                    :YourOpponent()
                    :Build(),
                1, true, 'DISCARDED'
            ),
            UM.Effects:BlindBoost(
                UM.Number:Static(1),
                UM.Select:Players()
                    :You()
                    :Build()
            )
        )
    :Build()
end