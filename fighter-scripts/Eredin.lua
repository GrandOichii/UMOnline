
function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            UM.Select:Fighters():Named('Eredin'):BuildPredicate(),
            UM.Mod.Cards:AllCards(UM.Number:Static(1)),
            UM.Conditions.CharacterSpecific:EredinEnraged()
        )
        :ModManoeuvreValue(
            UM.Select:Fighters():AllYour():BuildPredicate(),
            function (args, subjects, original)
                -- TODO too low-level
                if not UM.Conditions.CharacterSpecific:EredinEnraged()(args) then
                    return original
                end
                return 3
            end
        )

    :Build()
end

function UM.Conditions.CharacterSpecific:EredinEnraged()
    return UM.Conditions:CountEq(
        UM.Select:Fighters()
            :Undefeated()
            :Named('Red Rider')
            :Build(),
        0
    )
end