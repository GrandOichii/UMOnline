local UM = {}

-- Card creation

function UM:Card()
    -- TODO

    local result = {}

    function result:DuringCombat(TODO)
        -- TODO

        return result
    end

    function result:AfterCombat(TODO)
        -- TODO

        return result
    end

    function result:Immediately(TODO)
        -- TODO

        return result
    end

    function result:Effect(TODO)
        -- TODO

        return result
    end

    function result:Build()
        -- TODO
    end

    return result
end

-- Fighter creation

function UM:Fighter()
    local result = {}

    function result:Build()
        -- TODO
    end

    return result
end

-- Control flow

-- Effects

UM.Effects = {}

-- function UM.Effects:Custom(effectF)
--     -- TODO create some metadata
--     return function (args)
--         -- TODO store metadata
--         local meta = {}
--         effectF(args, meta)
--         -- TODO? return something
--     end
-- end

function UM.Effects:If(conditionalFunc, effectFunc)
    -- TODO
end

function UM.Effects:IfInstead(conditionalFunc, trueEffectFunc, falseEffectFunc)
    -- TODO
end

function UM.Effects:CancelAllEffectsOfOpponentsCard()
    -- TODO
end

function UM.Effects:Draw(manyPlayers, numeric)
    -- TODO
end

function UM.Effects:Move(manyFighters, upToAmount, canMoveOverOpposing)
    -- TODO
end

function UM.Effects:GainActions(fixedNumber)
    -- TODO
end

function UM.Effects:Discard(manyPlayers, fixedNumber, random)
    -- TODO
end

function UM.Effects:AllowBoost(numeric)
    -- TODO
end

function UM.Effects:DealDamage(manyfighters, numeric)
    -- TODO
end

function UM.Effects:PlaceFighter(singleFighter, manySpaces)
    -- TODO
end

function UM.Effects:PreventAllDamage()
    -- TODO prevent all damage in combat
end

function UM.Effects:ParalyzingFetters()
    -- The value of your opponent's card is equal to its printed value and cannot be changed
    -- TODO
end

function UM.Effects:ImpossibleToSee()
    -- The value of your opponent's attack or defense is 0 and cannot be changed by card effects. (Other card effects still happen.)
    -- TODO
end

-- Character-specific

UM.Effects.CS = {}

function UM.Effects.CS:ChangeSize()
    -- TODO
end

