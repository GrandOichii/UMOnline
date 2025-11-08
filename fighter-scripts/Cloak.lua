
function _Create()
    return UM.Build:Fighter()
        :AfterAttack(
            'After you attack, if Cloak dealt at least 2 combat damage, your opponent discards 1 card.',
            UM.Select:Fighters():Named('Cloak'):BuildPredicate(),
            UM.Effects:If(
                UM.Conditions:Gte(
                    UM.Combat:DamageDealt(),
                    UM.Number:Static(2)
                ),
                UM.Effects:Discard(
                    UM.Select:Players():YourOpponent():Build(), 1, false
                )
            )
        )
    :Build()
end