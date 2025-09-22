function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: Deal 1 damage to Foo.',
            UM.Effects:MoveFighters(
                UM.S:Fighters()
                :Build(),
                UM:UpTo(3)
            )
        )
        :Build()
end
