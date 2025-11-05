
-- After Angel or Faith attacks, if you lost the combat, draw 1 card.

function _Create()
    return UM.Build:Fighter()
        :AfterAttack(
            'After you attack, you may move your attacking fighter up to 2 spaces.',
            UM.Select:Fighters():Named('Angel', 'Faith'):BuildPredicate(),
            UM.Effects:If(
                UM.Conditions:CombatLostBy(UM.Player:EffectOwner()),
                UM.Effects:Draw(
                    UM.Select:Players():You():Build(),
                    UM.Number:Static(1),
                    false
                )
            )
        )
    :Build()
end
