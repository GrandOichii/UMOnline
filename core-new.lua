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

-- Single player


UM.Player = {}

function UM.Player:EffectOwner()
    return function (args)
        return args.owner
    end
end

-- function UM.Player:Opponent()
--     return function (args)
--         -- TODO
--     end
-- end

-- Single fighter

UM.Fighter = {}

function UM.Fighter:Source()
    return function (args)
        return args.fighter
    end
end

-- Card creation

UM.Build = {}

function UM.Build:Card()
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

function UM.Build:Fighter()
    local result = {}

    result.turnPhaseEffects = {}
    result.cardValueModifiers = {}
    result.whenPlaced = {}
    result.manoeuvreValueMods = {}

    function result:ModManoeuvreValue(fighterPredFunc, modFunc)
        result.manoeuvreValueMods[#result.manoeuvreValueMods+1] = {
            ['fighterPred'] = fighterPredFunc,
            ['modFunc'] = modFunc,
        }
        return result
    end

    function result:ModCardValue(modFunc, modCondition)
        modCondition = modCondition or function (...)
            return true
        end

        result.cardValueModifiers[#result.cardValueModifiers+1] = function (args, combatCard, resultValue)
            if not modCondition(args) then
                return resultValue
            end
            return modFunc(args, combatCard, resultValue)
        end

        return result
    end

    function result:WhenPlaced(text, ...)
        result.whenPlaced[#result.whenPlaced+1] = {
            text = text,
            effects = {...}
        }
        return result
    end

    function result:AddTurnPhaseEffects(step, text, effects)
        result.turnPhaseEffects[step] = {
            text = text,
            effects = effects
        }

        return result
    end

    function result:AtTheStartOfYourTurn(text, ...)
        return result:AddTurnPhaseEffects(UM.TurnPhaseTriggers.START, text, {...})
    end

    -- function result:DefinePlayerAttribute()
        
    -- end

    function result:Build()
        local fighter = {
            TurnPhaseEffects = result.turnPhaseEffects,
            CardValueModifiers = result.cardValueModifiers,
            WhenPlacedEffects = result.whenPlaced,
            ManoeuvreValueMods = result.manoeuvreValueMods,
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

    return function (...)
        return result
    end
end

function UM.Number:Static(v)
    return UM.Number:_({
        [1] = v
    })
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
    return function (args)
        if not conditionalFunc(args) then
            return
        end
        effectFunc(args)
    end
end

function UM.Effects:IfInstead(conditionalFunc, trueEffectFunc, falseEffectFunc)
    -- TODO
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

-- Conditions

UM.Conditions = {}

function UM.Conditions:PlayerAttributeEqualTo(attrKey, attrValue)
    return function (args)
        return GetPlayerAttribute(args.owner, attrKey) == attrValue
    end
end

function UM.Conditions:CombatWonBy(playerFunc)
    return function (args)
        local combat = GetCombat()
        -- TODO assert that combat is not null
        -- TODO assert that combat winner is not null
        return combat.Winner == playerFunc(args)
    end
end

function UM.Conditions:FightersCountGte(manyFighters, amount)
    return function (args)
        -- TODO feels wierd
        local fighters = manyFighters(args)
        return #fighters >= amount
    end
end

function UM.Conditions:Eq(numeric1, numeric2)
    return function (args)
        return numeric1(args):Last() == numeric2(args):Last()
    end
end

-- Counters

UM.Count = {}

function UM.Count:CardsInHand(singlePlayer)
    return function (args)
        local player = singlePlayer(args)
        return UM.Number:Static(GetHandSize(player))
    end
end

function UM.Count:Fighters(manyFighters)
    return function (args)
        return UM.Number:Static(#manyFighters(args))
    end
end

-- Card modifications

UM.Mod = {}
UM.Mod.Cards = {}

function UM.Mod.Cards:_(value, boostsAttackCards, boostsDefenseCards)
    return function (args, combatCard, result)
        if not boostsAttackCards and not combatCard.IsDefence then
            return result
        end
        if not boostsDefenseCards and combatCard.IsDefence then
            return result
        end
        return result + value
    end
end

function UM.Mod.Cards:AttackCards(value)
    return UM.Mod.Cards:_(value, true, false)
end

function UM.Mod.Cards:DefenseCards(value)
    return UM.Mod.Cards:_(value, false, true)
end

-- Effects

function UM.Effects:Draw(manyPlayers, numeric, optional)
    return function (args)
        local players = manyPlayers(args)
        for _, p in ipairs(players) do
            local amount = numeric(args):Choose(args, 'Choose how many cards to draw')
            DrawCards(p, amount)
        end
    end
end

function UM.Effects:MoveFighters(manyFighters, numeric, canMoveOverOpposing)
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
            local amount = numeric(args):Choose(args, 'Choose how much damage to deal to '..fighter.Name)
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

function UM.Effects:CancelAllEffectsOnOpponentsCard()
    return function (args)
        local player = args.owner
        CancelCombatEffectsOfOpponent(player)
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


-- entity selectors

UM.Select = {}

function UM.Select:CardsInDiscardPile(ofPlayer)
    local result = {}

    result.filters = {}

    function result:Count()
        return function (args)
            return #result:_Select(args)
        end
    end

    function result:_Select(args)
        local player = ofPlayer(args)
        local allCards = GetCardsInDiscardPile(player)
        local cards = {}

        local filterFunc = function (card)
            for _, filter in ipairs(result.filters) do
                if not filter(args, card) then
                    return false
                end
            end
            return true
        end

        for _, card in ipairs(allCards) do
            if filterFunc(card) then
                cards[#cards+1] = card
            end
        end

        -- TODO check for 0

        -- if result.single then
        --     local fighter = fighters[1]

        --     if #fighters > 1 then
        --         fighter = ChooseFighter(args.owner, fighters, 'Choose a fighter')
        --     end

        --     fighters = {
        --         [1] = fighter
        --     }

        -- end

        return cards
    end

    function result:_Add(func)
        result.filters[#result.filters+1] = func

        return result
    end

    function result:WithLabel(label)
        return result:_Add(function (args, card)
            return CardHasLabel(card, label)
        end)
    end

    function result:Build()
        return function (args)
            return result:_Select(args)
        end
    end

    return result
end

function UM.Select:Fighters()
    local result = {}

    result.filters = {}
    result.single = false

    -- function result:OwnedBy(playerFunc)
    --     result.filters[#result.filters+1] = function (args, fighter)
    --         return fighter.Owner.Idx == playerFunc(args).Idx
    --     end

    --     return result
    -- end

    function result:OwnedBy(playerFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            return fighter.Owner.Idx == playerFunc(args).Idx
        end

        return result
    end

    function result:AllYour()
        return result:OwnedBy(UM.Player:EffectOwner())
    end

    function result:Your()
        -- TODO

        return result
    end

    function result:Named(name)
        result.filters[#result.filters+1] = function (args, fighter)
            return IsCalled(fighter, name)
        end

        return result
    end

    function result:InCombat()
        result.filters[#result.filters+1] = function (args, fighter)
            return IsInCombat(fighter)
        end
        
        return result
    end

    function result:Only(fighterFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            local f = fighterFunc(args)
            if not IsAlive(f) then
                return false
            end
            return fighter == f
        end

        return result
    end

    function result:AdjacentTo(fighterFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            local f = fighterFunc(args)
            if not IsAlive(f) then
                return false
            end
            return AreAdjacent(fighter, f)
        end

        return result
    end

    function result:InSameZoneAs(fighterFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            local f = fighterFunc(args)
            if not IsAlive(f) then
                return false
            end
            return AreInSameZone(fighter, f)
        end

        return result
    end

    function result:OpposingInCombatTo(fighterFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            local f = fighterFunc(args)
            if not IsAlive(f) then
                return false
            end
            return AreOpposingInCombat(fighter, f)
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

    function result:_Select(args)
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

    function result:Build()
        return function (args)
            return result:_Select(args)
        end
    end

    function result:BuildOne()
        return function (args)
            -- TODO
        end
    end

    function result:YourFighter()
        -- TODO

        return result
    end

    return result
end


function UM.Select.Players()
    local result = {}

    result.filters = {}
    result.single = false

    -- function result:OpposingTo(playerFunc)
    --     -- TODO
    --     result.filters[#result.filters+1] = function (args, player)
    --         return AreOpposingPlayers(player, playerFunc(args))
    --     end
    --     return result
    -- end

    function result:_Select(args)
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

        if result.single then
            local player = players[1]

            if #players > 1 then
                player = ChoosePlayer(args.owner, players, 'Choose a player')
            end

            players = {
                [1] = player
            }

        end

        return players
    end

    -- function result:BuildOne()
    --     return function (args)
    --         -- TODO is this the right way
    --         local options = result:_Select(args)
    --         local player = options[1]
    --         -- if #options > 1 then
    --         --     fighter = ChooseFighter(args.owner, options, 'Choose a single fighter')
    --         -- end
    --         return player
    --     end
    -- end

    -- function result:Single()
    --     result.single = true
    --     return result
    -- end

    function result:You()
        -- TODO

        return result
    end

    function result:YourOpponent()
        -- TODO

        return result
    end

    function result:Build()
        return function (args)
            return result:_Select(args)
        end
    end

    return result
end


-- Character-specific

UM.Effects.CharacterSpecific = {}
