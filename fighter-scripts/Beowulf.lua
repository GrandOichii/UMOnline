
-- Beowulf starts with 1 Rage.
-- When Beowulf is dealt damage, he gains 1 Rage.
-- Beowulf has a maximum of 3 rage.

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Beowulf starts with 1 Rage.',
            UM.Effects.CharacterSpecific:SetRage(1)
        )
        :OnDamage(
            'When Beowulf is dealt damage, he gains 1 Rage.',
            UM.Effects.CharacterSpecific:AddRage(1)
        )
    :Build()
end

function UM.Effects.CharacterSpecific:SetRage(value)
    return function (args)
        local prev = GetPlayerIntAttribute(args.owner, 'RAGE')
        if prev == value then
            return
        end
        if value > 3 then
            value = 3
        end
        if value < 0 then
            value = 0
        end
        SetPlayerIntAttribute(args.owner, 'RAGE', value)
        LogPublic('Beowulf\'s rage is set to '..tostring(value))
    end
end

function UM.Effects.CharacterSpecific:AddRage(value)
    return function (args)
        LogPublic('Beowulf\'s gains '..value..'rage!')
        UM.Effects.CharacterSpecific:SetRage(GetPlayerIntAttribute(args.owner, 'RAGE') + value)(args)
    end
end

function UM.Count.CharacterSpecific:Rage()
    return function (args)
        return UM.Number:Static(GetPlayerIntAttribute(args.owner, 'RAGE'))
    end
end