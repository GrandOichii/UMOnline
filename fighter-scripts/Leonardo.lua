function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, move any fighter up to 1 space.',
            UM.Effects:MoveFighters(
                UM.Select:Fighters():Single():Build(),
                UM.Number:UpTo(1),
                false
            )
        )
    :Build()
end