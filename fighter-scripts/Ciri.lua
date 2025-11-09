-- (7) Effects on Ciri's cards cannot be canceled.

function _Create()
    return UM.Build:Fighter()
        :ForbidCardCancelling(
            function (args, card)
                -- TODO too low level
                
                if not IsCardOfCharacter(card, 'Ciri') then
                    DEBUG('NOT CIRI')
                    return false
                end
                if UM.Count.CharacterSpecific:CiriSource()(args) < 7 then
                    DEBUG(tostring(UM.Count.CharacterSpecific:CiriSource()(args)))
                    return false
                end
                DEBUG('TRUE')
                return true
            end,
            UM.Select:Players():BuildPredicate()
        )
    :Build()
end

function UM.Count.CharacterSpecific:CiriSource()
    return UM.Select:CardsInDiscardPile(UM.Player:EffectOwner())
        :WithLabel('source')
        :Count()
end

function UM.Effects.CharacterSpecific:SourceEffect(source, text, ...)
    local effects = {...}
    return {
        ['source'] = source,
        ['text'] = text,
        ['effect'] = function (args)
            for _, e in ipairs(effects) do
                e(args)
            end
        end
    }
end

function UM.Effects.CharacterSpecific:SourceSwitch(...)
    local sourceEffects = {...}
    return function (args)
        local source = UM.Count.CharacterSpecific:CiriSource()(args)
        local result = nil
        for _, e in ipairs(sourceEffects) do
            if source > e.source then
                result = e
            end
        end
        if result == nil then
            -- TODO log that no source effect was applied
            return
        end

        -- TODO log that source effect of result.source 
        result.effect(args)
    end
end