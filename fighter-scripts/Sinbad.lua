
-- When you maneuver, you may move your fighters +1 space for each VOYAGE card in your discard pile.

function _FighterIn(manyFighters)
    return function (args, fighter)
        local fighters = manyFighters(args)
        for _, v in ipairs(fighters) do
            if v == fighter then
                return true
            end
        end
        return false
    end
end

function _Create()
    return UM.Build:Fighter()
        :ModManoeuvreValue(
            _FighterIn(UM.Select:Fighters():AllYour():Build()),
            -- TODO too low-level
            function (args, value)
                LogPublic(tostring(value)..' '..tostring(value + 1))
                return value + 1
            end
        )
    :Build()
end