
-- Raptors add 1 to the value of their attack cards for each of your other Raptors adjacent to the defender.

function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            UM.Select:Fighters()
                :Named('Raptor')
                :Except(UM.Fighter:Source())
                :BuildPredicate(),
            UM.Mod.Cards:AttackCards(
                UM.Number:Static(1)
            ),
            UM.Conditions:FightersAreAdjacent(
                UM.Fighter:Source(),
                UM.Fighter:Defender()
            )
        )
    :Build()
end