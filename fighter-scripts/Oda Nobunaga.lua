
-- Other friendly fighters in Oda Nobunaga's zone add +1 to the value of their played combat cards. (Oda Nobunaga does not benefit from this ability.)

function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            UM.Select:Fighters()
                :Except(UM.Fighter:Named('Oda Nobunaga'))
                :InSameZoneAs(UM.Fighter:Named('Oda Nobunaga'))
                :Friendly()
                :BuildPredicate(),
            UM.Mod.Cards:AllCards(UM.Number:Static(1))
        )
    :Build()
end