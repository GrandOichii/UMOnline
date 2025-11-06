
-- Luke Cage takes 2 less combat damage from attacks. 

-- TODO too low-level
function _Create()
    return UM.Build:Fighter()
        :ModifyDamage(
            function (args, fighter, isCombatDamage, damage)
                if not isCombatDamage then
                    DEBUG('NOT COMBAT DAMAGE')
                    return damage
                end
                if not UM.Select:Fighters():Named('Luke Cage'):BuildPredicate()(args, fighter) then
                    DEBUG('NOT LUKE CAGE')
                    return damage
                end
                DEBUG(tostring(damage)..' '..tostring(damage-2))
                return damage - 2
            end
        )
    :Build()
end