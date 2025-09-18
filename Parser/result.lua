function _Create()
    return UM.Card()
        :AfterCombat(
            'After combat: Draw up to 2 cards. Gain 1 action.',
            UM.Effects:Draw(
                UM.UpTo(2)
            ),
            UM.Effects:GainActions(
                UM:Static(1)
            )
        )
        :Build()
end
