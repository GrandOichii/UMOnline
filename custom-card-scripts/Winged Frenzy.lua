function _Create()
    return UM.Build:Card()
        :Effect(
            'Move each of your fighters up to 3 spaces. You may move them through spaces containing opposing fighters. Then, return a defeated Harpy (if any) to any space in Medusa\'s zone.',
            UM.Effects:MoveFighters(
                UM.Select:Fighters()
                    :Your()
                    :Build(),
                UM.Number:UpTo(3),
                true
            )
            -- TODO resurrection
        )
        :Build()
end
