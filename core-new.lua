local UM = {}

UM.CombatSteps = {
    IMMEDIATELY = 0,
    DURING_COMBAT = 1,
    AFTER_COMBAT = 2,
}

UM.TurnPhaseTriggers = {
    START = 0,
    END = 1,
}


-- Card creation

function UM:Card()
    local result = {}

    result.scheme = {
        text = '',
        effects = {},
    }

    result.combatStepEffects = {}

    function result:CombatStepEffect(step, text, ...)
        local obj = result.combatStepEffects[step]
        -- assert(obj == nil, 'Already defined effects for combat step'..step..'('..obj.text..')')
        obj = {
            text = '',
            effects = {}
        }
        obj.text = text
        local effects = {...}

        for _, v in ipairs(effects) do
            obj.effects[#obj.effects+1] = v
        end
        result.combatStepEffects[step] = obj

        return result
    end

    function result:Immediately(text, ...)
        return result:CombatStepEffect(UM.CombatSteps.IMMEDIATELY, text, ...)
    end

    function result:DuringCombat(text, ...)
        return result:CombatStepEffect(UM.CombatSteps.DURING_COMBAT, text, ...)
    end

    function result:AfterCombat(text, ...)
        return result:CombatStepEffect(UM.CombatSteps.AFTER_COMBAT, text, ...)
    end

    function result:Effect(text, ...)
        local effects = {...}

        result.scheme.text = text
        result.scheme.effects = {}

        for _, v in ipairs(effects) do
            result.scheme.effects[#result.scheme.effects+1] = v
        end

        return result
    end

    function result:Build()
        return {
            Scheme = result.scheme,
            CombatStepEffects = result.combatStepEffects,
        }
    end

    return result
end

-- Fighter creation

function UM:Fighter()
    local result = {}

    result.turnPhaseEffects = {}

    function result:AddTurnPhaseEffects(step, text, ...)
        local obj = {
            text = '',
            effects = {}
        }
        obj.text = text
        local effects = {...}

        for _, v in ipairs(effects) do
            obj.effects[#obj.effects+1] = v
        end
        result.turnPhaseEffects[step] = obj

        return result
    end

    function result:AtTheStartOfYourTurn(text, ...)
        return result:AddTurnPhaseEffects(UM.TurnPhaseTriggers.START, text, ...)
    end

    -- function result:DefinePlayerAttribute()
        
    -- end

    function result:Build()
        local fighter = {
            TurnPhaseEffects = result.turnPhaseEffects
        }
        return fighter
    end

    return result
end

-- numeric

UM.Number = {}

function UM.Number:_(options)
    local result = {
        values = options
    }

    function result:Choose(args, hint)
        local _result = result.values[1]
        if #result.values > 1 then
            _result = ChooseNumber(args.owner, result.values, hint)
        end
        return _result
    end

    function result:Last()
        return result.values[#result.values]
    end

    return result
end

function UM.Number:Static(v)
    return UM.Number:_(v)
end

function UM.Number:UpTo(max)
    local values = {}
    for i = 1, max do
        values[#values+1] = i
    end
    return UM.Number:_(values)
end

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

-- Control flow

function UM.Effects:If(conditionalFunc, effectFunc)
    -- TODO
end

function UM.Effects:IfInstead(conditionalFunc, trueEffectFunc, falseEffectFunc)
    -- TODO
end

function UM.Effects:CancelAllEffectsOfOpponentsCard()
    return function (args)
        local player = args.owner
        CancelCombatEffectsOfOpponent(player)
    end
end

-- Effects

function UM.Effects:Draw(manyPlayers, numeric, optional)
    return function (args)
        local fighter = args.fighter
        local amount = numeric:Choose(args, 'Choose how many cards to draw')
        DrawCards(fighter.Owner, amount)
    end
end

function UM.Effects:Move(manyFighters, numeric, canMoveOverOpposing)
    return function (args)
        local fighters = manyFighters(args)
        local amount = numeric(args):Last()
        MoveFighters(args.owner, fighters, amount, canMoveOverOpposing)
    end
end

function UM.Effects:GainActions(fixedNumber)
    return function (args)
        local fighter = args.fighter
        GainActions(fighter.Owner.Idx, fixedNumber)
    end
end

function UM.Effects:Discard(manyPlayers, fixedNumber, random)
    local discardCards = function (player, amount, cardIdxFunc)
        while amount > 0 do
            local idx = cardIdxFunc()
            DiscardCard(player, idx)
            amount = amount - 1
        end
    end

    return function (args)
        local players = manyPlayers(args)
        local amount = fixedNumber

        for _, player in ipairs(players) do
            if random then
                discardCards(player, amount, function ()
                    return Rnd(GetHandSize(player))
                end)
            else
                discardCards(player, amount, function ()
                    return ChooseCardInHand(player, player, 'Choose a card to discard')
                end)
            end
        end
    end

end

function UM.Effects:AllowBoost(numeric, optional)
    -- TODO
end

function UM.Effects:DealDamage(manyfighters, numeric)
    return function (args)
        local fighters = manyfighters(args)

        for _, fighter in ipairs(fighters) do
            local amount = numeric:Choose(args, 'Choose how much damage to deal to '..fighter.Name)
            DealDamage(fighter, amount)
        end
    end
end

function UM.Effects:PlaceFighter(singleFighter, manySpaces)
    return function (args)
        local fighter = singleFighter(args)
        local nodes = manySpaces(args)
        local options = {}
        for _, node in ipairs(nodes) do
            if node.Fighter == nil or node.Fighter == fighter then
                options[#options+1] = node
            end
        end
        if #options == 0 then
            return
        end
        local node = options[1]
        if #options > 1 then
            node = ChooseNode(args.owner, options, 'Choose where to place '..fighter.LogName)
        end
        PlaceFighter(fighter, node)
    end
end

function UM.Effects:RecoverHealth(manyFighters, amount)
    return function (args)
        local fighters = manyFighters(args)

        for _, fighter in ipairs(fighters) do
            RecoverHealth(fighter, amount)
        end
    end
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

UM.Effects.CharacterSpecific = {}
