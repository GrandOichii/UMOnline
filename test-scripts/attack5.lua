function _Create()
    return UM:Card()
        :DuringCombat(
            'You may BOOST this attack.',
            UM.Effects:AllowOptionalBoost(
                UM:Static(1)
            )
        )
        :Build()
end
