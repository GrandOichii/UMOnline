
-- When an opponent attacks Spider-Man, before you play a defense card, they must tell you the printed value of their card.
function _Create()
    return UM.Build:Fighter()
        :OnCombatCardChoice(
            'When an opponent attacks Spider-Man, before you play a defense card, they must tell you the printed value of their card.',
            function (args, subjects)
                -- TODO too low-level
                local combatPart = subjects.combatPart
                if not UM.Conditions:IsDefender(UM.Player:EffectOwner())(args) then
                    return
                end
                
                LogPublic('The value of the attack card is '..combatPart.Value)
            end
        )
    :Build()
end