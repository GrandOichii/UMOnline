-- TODO? is optional necessary

function _Create()
    return UM.Build:Fighter()
        :AtTheEndOfYourTurn(
            'At the end of your turn, you may move Bruce Lee 1 space.',
            UM.Effects:Optional(
                'Move Bruce Lee 1 space?',
                UM.Effects:MoveFighters(
                    UM.Select:Fighters():Named('Bruce Lee'):Build(),
                    UM.Number:Static(1),
                    false
                )
            )
        )
    :Build()
end
