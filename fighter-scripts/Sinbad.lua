
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
            UM.Select:Fighters():AllYour():BuildPredicate(),
            function (args, original)
                -- TODO too low-level
                return original + UM.Select:CardsInDiscardPile(UM.Player:EffectOwner())
                    :WithLabel('voyage')
                    :Count()(args)
            end
        )
    :Build()
end