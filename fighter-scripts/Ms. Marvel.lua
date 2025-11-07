function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may move Ms. Marvel 1 space.',
            UM.Effects:MoveFighters(
                UM.Select:Fighters():Named('Ms. Marvel'):Build(),
                UM.Number:UpTo(1),
                false
            )
        )
    :Build()
end