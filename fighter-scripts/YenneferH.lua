
function _Create()
    return UM.Build:Fighter()
        :Immediately(
            'If Yennefer is attacking, you may BOOST her attack. (This effect cannot be canceled.)',
            UM.Effects:Optional(
                'Boost your attack?',
                UM.Effects:AllowBoost(
                    UM.Number:Static(1),
                    false
                )
            )
        )
    :Build()
end