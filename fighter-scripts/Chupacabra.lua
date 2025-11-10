-- 

function _Create()
    return UM.Build:Fighter()
        :AfterAttack(
            'After you attack, you may draw a card.',
            UM.Select:Fighters():Your():Build(),
            UM.Effects:Optional(
                'Draw a card?',
                UM.Effects:Draw(
                    UM.Select:Players():You():Build(),
                    UM.Number:Static(1), false
                )
            )
        )
    :Build()
end