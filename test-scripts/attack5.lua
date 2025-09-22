function _Create()
    return UM:Card()
        :AfterCombat(
            'After combat: Move each of your fighters up to 2 spaces.',
            UM.Effects:MoveFighters(
                UM.S:Fighters()
                :OwnedBy(UM.Players:EffectOwner())
                :Build(),
                UM:UpTo(2)
            )
        )
        :Build()
end
