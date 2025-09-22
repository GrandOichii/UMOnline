UM = {}

UM.CombatSteps = {
    IMMEDIATELY = 0,
    DURING_COMBAT = 1,
    AFTER_COMBAT = 2,
}

UM.TurnPhaseTriggers = {
    START = 0,
    END = 1,
}

function UM:Card()
    local result = {}

    result.scheme = {
        text = '',
        effects = {},
    }

    result.combatStepEffects = {}

    function result:Effect(text, ...)
        -- assert(result.scheme == nil, 'Already defined scheme effects ('..result.scheme.text..')')

        result.scheme.text = text

        for _, v in ipairs(result.scheme.effects) do
            result.schemeEffects[#result.schemeEffects+1] = v
        end

        return result
    end

    function result:CombatStepEffect(step, text, ...)
        local obj = result.combatStepEffects[step]
        -- TODO dont know is this is the best way of doing this
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

    function result:Build()
        return {
            Scheme = result.scheme,
            CombatStepEffects = result.combatStepEffects,
        }
    end

    return result
end

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

    function result:Build()
        local fighter = {
            TurnPhaseEffects = result.turnPhaseEffects
        }
        return fighter
    end

    return result
end

function UM:Static(amount)
    return function (...)
        return {
            [1] = amount
        }
    end
end

function UM:UpTo(amount)
    return function (...)
        local result = {}
        for i = 1, amount do
            result[#result+1] = i
        end
        return result
    end
end

function UM:If(conditionFunc, effectFunc)
    return function (args)
        if not conditionFunc(args) then
            return
        end
        effectFunc(args)
    end
end

function UM:IfInstead(conditionFunc, ifTrueEffectFunc, ifFalseEffectFunc)
    return function (args)
        if not conditionFunc(args) then
            ifFalseEffectFunc(args)
            return
        end
        ifTrueEffectFunc(args)
    end
end

UM.Players = {}

function UM.Players:EffectOwner()
    return function (args)
        return args.owner
    end
end

function UM.Players:Opponent()
    return function (args)
        local players = UM.S:Players()
            :OpposingTo(UM.Players:EffectOwner())
            :Build()(args)
        assert(#players > 0, 'No opposing players found')
        local result = players[1]
        if #players > 0 then
            -- TODO choose player
        end
        return {
            [1] = result
        }
    end
end

UM.Fighters = {}

function UM.Fighters:Source()
    return function (args)
        return args.fighter
    end
end

UM.Conditional = {}

function UM.Conditional:CombatWonBy(playerFunc)
    return function (args)
        local combat = GetCombat()
        -- TODO assert that combat is not null
        -- TODO assert that combat winner is not null
        return combat.Winner == playerFunc(args)
    end
end

function UM.Conditional:CombatLostBy(playerFunc)
    return function (args)
        local combat = GetCombat()
        -- TODO assert that combat is not null
        -- TODO assert that combat winner is not null
        -- TODO what about other players
        return combat.Winner ~= playerFunc(args)
    end
end

UM.Effects = {}

function NumericChoose(args, amounts, hint)
    assert(#amounts > 0, 'Provided empty table for NumericChoose')
    if #amounts == 1 then
        return amounts[1]
    end

    -- TODO prompt player to choose
    return ChooseNumber(args.owner, amounts, hint)
end

function UM.Effects:Discard(playerSelectorFunc, amountFunc, random)
    local discardCards = function (player, amount, cardIdxFunc)
        while amount > 0 do
            -- local 
            local hand = player.Hand.Cards
            local idx = cardIdxFunc()
            DiscardCard(player, idx)
            amount = amount - 1
        end
    end

    return function (args)
        local players = playerSelectorFunc(args)
        local amount = NumericChoose(args, amountFunc(args), 'Choose how many cards to discard')

        for _, player in ipairs(players) do
            if random then
                discardCards(player, amount, function ()
                    return Rnd(player.hand.Count)
                end)
            else
                discardCards(player, amount, function ()
                    return ChooseCardInHand(player, player, 'Choose a card to discard')
                end)
            end
        end
    end
end

function UM.Effects:Draw(amountFunc)
    return function (args)
        local fighter = args.fighter
        local amount = NumericChoose(args, amountFunc(args), 'Choose how many cards to draw')
        DrawCards(fighter.Owner.Idx, amount)
    end
end

function UM.Effects:MoveFighters(fighterSelectorFunc, amountFunc)
    return function (args)
        local fighters = fighterSelectorFunc(args)
        -- TODO feels weird
        local amounts = amountFunc(args)
        local amount = amounts[#amounts]

        for _, fighter in ipairs(fighters) do
            MoveFighter(fighter, amount)
        end
    end
end

function UM.Effects:GainActions(amountFunc)
    return function (args)
        local fighter = args.fighter
        local amount = NumericChoose(args, amountFunc(args), 'Choose how many actions to gain')
        GainActions(fighter.Owner.Idx, amount)
    end
end

function UM.Effects:DealDamage(amountFunc, fighterSelectorFunc)
    return function (args)
        local fighters = fighterSelectorFunc(args)

        for _, fighter in ipairs(fighters) do
            local amount = NumericChoose(args, amountFunc(args), 'Choose how much damage to deal to '..fighter.Name)
            DealDamage(fighter, amount)
        end
    end
end

function UM.Effects:PlaceFighter(fightersFunc, placeSelectorFunc)
    return function (args)
        local fighters = fightersFunc(args)
        assert(#fighters == 1, 'TODO fix this - provided more than 1 fighter for PlaceFighter')
        local fighter = fighters[1]

        local nodes = placeSelectorFunc(args)
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

function UM.Effects:RecoverHealth(fighterSelectorFunc, amountFunc)
    return function (args)
        local fighters = fighterSelectorFunc(args)

        for _, fighter in ipairs(fighters) do
            local amount = NumericChoose(args, amountFunc(args), 'Choose how much damage to deal to '..fighter.Name)
            RecoverHealth(fighter, amount)
        end
    end
end

function UM.Effects:Optional(hint, ...)
    local effectFuncs = {...}

    return function (args)
        local choice = ChooseString(args.owner, {
            [1] = 'Yes',
            [2] = 'No'
        }, hint)

        if choice == 'Yes' then
            for _, effectFunc in ipairs(effectFuncs) do
                effectFunc(args)
            end
        end
    end
end

-- entity selectors

UM.S = {}

function UM.S:Fighters()
    local result = {}

    result.filters = {}
    result.single = false

    function result:OwnedBy(playerFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            return fighter.Owner.Idx == playerFunc(args).Idx
        end

        return result
    end

    function result:Named(name)
        result.filters[#result.filters+1] = function (args, fighter)
            return IsCalled(fighter, name)
        end

        return result
    end

    function result:Only(fighterFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            return fighter == fighterFunc(args)
        end

        return result
    end

    function result:AdjacentTo(fighterFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            local f = fighterFunc(args)
            return AreAdjacent(fighter, f)
        end

        return result
    end

    -- function result:NotOwnedBy(playerFunc)
    --     result.filters[#result.filters+1] = function (args, fighter)
    --         return fighter.Owner.Idx ~= playerFunc(args).Idx
    --     end

    --     return result
    -- end

    function result:OpposingTo(playerFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            return IsOpposingTo(fighter, playerFunc(args))
        end

        return result
    end

    function result:Single()
        result.single = true
        return result
    end

    function result:Build()
        return function (args)
            local allFighters = GetFighters()
            local fighters = {}

            local filterFunc = function (fighter)
                for _, filter in ipairs(result.filters) do
                    if not filter(args, fighter) then
                        return false
                    end
                end
                return true
            end

            for _, fighter in ipairs(allFighters) do
                if filterFunc(fighter) then
                    fighters[#fighters+1] = fighter
                end
            end

            -- TODO check for 0

            if result.single then
                local fighter = fighters[1]

                if #fighters > 1 then
                    fighter = ChooseFighter(args.owner, fighters, 'Choose a fighter')
                end

                fighters = {
                    [1] = fighter
                }

            end

            return fighters
        end
    end

    return result
end

function UM.S.Players()
    local result = {}

    result.filters = {}

    function result:OpposingTo(playerFunc)
        result.filters[#result.filters+1] = function (args, player)
            return AreOpposingPlayers(player, playerFunc(args))
        end
        return result
    end

    function result:Build()
        return function (args)
            local allPlayers = GetPlayers()
            local players = {}

            local filterFunc = function (player)
                for _, filter in ipairs(result.filters) do
                    if not filter(args, player) then
                        return false
                    end
                end
                return true
            end

            for _, player in ipairs(allPlayers) do
                if filterFunc(player) then
                    players[#players+1] = player
                end
            end

            return players
        end
    end

    return result
end

function UM.S.Spaces()
    local result = {}
    result.filters = {}

    function result:InSameZoneAs(fighterFunc)
        result.filters[#result.filters+1] = function (args, space)
            local fighter = fighterFunc(args)
            local zones = GetFighterZones(fighter)
            return IsInZone(space, zones)
        end
        return result
    end

    function result:Empty()
        result.filters[#result.filters+1] = function (args, space)
            -- TODO tokens
            return space.Fighter == nil
        end
        return result
    end

    function result:Build()
        return function (args)
            local allNodes = GetNodes()
            local nodes = {}

            local filterFunc = function (node)
                for _, filter in ipairs(result.filters) do
                    if not filter(args, node) then
                        return false
                    end
                end
                return true
            end

            for _, node in ipairs(allNodes) do
                if filterFunc(node) then
                    nodes[#nodes+1] = node
                end
            end

            return nodes
        end
    end

    return result
end