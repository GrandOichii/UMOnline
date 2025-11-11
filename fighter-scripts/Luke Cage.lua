
-- Luke Cage takes 2 less combat damage from attacks. 

-- TODO too low-level
function _Create()
    return UM.Build:Fighter()
        :ModifyDamage(
            function (args, fighter, isCombatDamage, damage)
                if not isCombatDamage then
                    return damage
                end
                if not UM.Select:Fighters():Named('Luke Cage'):BuildContains()(args, fighter) then
                    return damage
                end
                return damage - 2
            end
        )
    :Build()
end