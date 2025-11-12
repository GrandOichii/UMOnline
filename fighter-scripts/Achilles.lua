
function _Create()
    return UM.Build:Fighter()
        -- ? should this be declared in Patroclus.lua
        :OnFighterDefeat(
            'When Patroclus is defeated, discard 2 random cards.',
            UM.Select:Fighters():Named('Patroclus'):BuildPredicate(),
            UM.Effects:Discard(
                UM.Select:Players():You():Build(), 2, true
            )
        )
        :ModCardValue(
            'Add +2 to the value of all Achilles\' attacks',
            UM.Select:Fighters():Named('Achilles'):BuildPredicate(),
            UM.Mod.Cards:AttackCards(UM.Number:Static(2)),
            UM.Conditions:FightersAreDefeated(
                UM.Select:Fighters():Named('Patroclus'):Build()
            )
        )
        :AfterCombat(
            'If Achilles wins combat, draw 1 card.',
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions:CombatWonBy(UM.Player:EffectOwner()),
                    UM.Conditions:FightersAreDefeated(
                        UM.Select:Fighters():Named('Patroclus'):Build()
                    )
                ),
                UM.Effects:Draw(
                    UM.Select:Players():You():Build(),
                    UM.Number:Static(1),
                    false
                )
            )
        )
    :Build()
end