function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: Draw 1 card.',
            UM.Effects:Draw(
                UM:Static(1)
            )
        )
        :Build()
end
