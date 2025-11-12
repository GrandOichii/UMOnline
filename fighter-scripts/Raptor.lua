
function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            'Raptors add 1 to the value of their attack cards for each of your other Raptors adjacent to the defender.',
            UM.Select:Fighters()
                :Source()
                :BuildPredicate(),
            UM.Mod.Cards:AttackCards(
                UM.Number:Count(
                    UM.Select:Fighters()
                        :AdjacentTo(UM.Fighter:Defender())
                        :Except(UM.Fighter:Source())
                        :Build()
                )
            ),
            UM.Conditions:FightersAreAdjacent(
                UM.Fighter:Source(),
                UM.Fighter:Defender()
            )
        )
    :Build()
end