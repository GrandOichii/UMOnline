
-- After you attack, you may move your attacking fighter up to 2 spaces.
-- (This applies to all attackers, including outlaws)

function _Create()
    return UM.Build:Fighter()
        :AfterAttack(
            'After you attack, you may move your attacking fighter up to 2 spaces.',
            UM.Select:Fighters():AllYour():FighterPredicate(),
            UM.Effects:MoveFighters(
                UM.Select:Fighters():Your():Build(),
                UM.Number:UpTo(2),
                false
            )
        )
    :Build()
end
