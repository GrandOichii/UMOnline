function _Create()
    return UM:Card()
        :Effect(
            'Move each of your fighters up to 3 spaces. You may move them through spaces containing opposing fighters. Then, return a defeated Harpy (if any) to any space in Medusa\'s zone.',
            UM.Effects:MoveFighters(
                UM.S:Fighters()
                    :OwnedBy(UM.Player:EffectOwner())
                    :Build(),
                UM:UpTo(3),
                true
            )
            -- TODO resurrection
        )
        :Build()
end
