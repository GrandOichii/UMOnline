
function _Create()
    return UM.Build:Fighter()
        :AfterAttack(
            'After you attack, if Dagger dealt at least 2 combat damage, gain 1 action.',
            UM.Select:Fighters():Named('Dagger'):BuildPredicate(),
            UM.Effects:If(
                UM.Conditions:Gte(
                    UM.Combat:DamageDealt(),
                    UM.Number:Static(2)
                ),
                UM.Effects:GainActions(1)
            )
        )
    :Build()
end