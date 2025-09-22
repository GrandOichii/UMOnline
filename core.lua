UM = {}

UM.CombatSteps = {
    IMMEDIATELY = 0,
    DURING_COMBAT = 1,
    AFTER_COMBAT = 2,
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
        -- print(obj)
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

function UM:Static(amount)
    return function (...)
        return amount
    end
end

function UM:UpTo(amount)
    -- TODO change this
    return function (...)
        return amount
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

UM.Players = {}

function UM.Players:EffectOwner()
    return function (args)
        return args.fighter.Owner
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
        local amount = amountFunc(args)

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
        local amount = amountFunc(args)
        DrawCards(fighter.Owner.Idx, amount)
    end
end

function UM.Effects:MoveFighters(fighterSelectorFunc, amountFunc)
    return function (args)
        local fighters = fighterSelectorFunc(args)
        local amount = amountFunc(args)
        for _, fighter in ipairs(fighters) do
            print(fighter)
            print(amount)
            MoveFighter(fighter, amount)
        end
    end
end

function UM.Effects:GainActions(amountFunc)
    return function (args)
        local fighter = args.fighter
        local amount = amountFunc(args)
        GainActions(fighter.Owner.Idx, amount)
    end
end

function UM.Effects:DealDamage(selectorFunc, amountFunc)
    return function (args)
        local fighters = selectorFunc(args)
        local amount = amountFunc(args)

        for _, fighter in ipairs(fighters) do
            DealDamage(fighter, amount)
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

    function result:NotOwnedBy(playerFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            return fighter.Owner.Idx ~= playerFunc(args).Idx
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

            if result.single then
                local single = ChooseFighter(args.fighter.Owner, fighters, 'Choose a fighter')
                fighters = {
                    [1] = single
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