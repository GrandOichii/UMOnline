
-- Raptors add 1 to the value of their attack cards for each of your other Raptors adjacent to the defender.

function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            UM.Mod.Cards:AttackCards(
                UM.Number:Count(
                    UM.Select:Fighters()
                        :Named('Raptor')
                        :Except(UM.Fighter:Source())
                        :AdjacentTo(UM.Fighter:Defender())
                        :Build()
                )
            )
        )
    :Build()
end