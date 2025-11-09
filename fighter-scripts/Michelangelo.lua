
-- After you attack or scheme, draw 1 card.
-- Your starting and maximum hand size is 3.

function _Create()
    return UM.Build:Fighter()
        :AfterAttack(
            'After you attack, draw 1 card.',
            UM.Select:Fighters():AllYour():BuildPredicate(),
            UM.Effects:Draw(
                UM.Select:Players():You():Build(),
                UM.Number:Static(1), false
            )
        )
        :AfterScheme(
            'After you scheme, draw 1 card.',
            UM.Select:Fighters():AllYour():BuildPredicate(),
            UM.Effects:Draw(
                UM.Select:Players():You():Build(),
                UM.Number:Static(1), false
            )
        )
    :Build()
end