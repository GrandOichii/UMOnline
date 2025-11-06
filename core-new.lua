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
--         -- TODO combat opponent
--     end
-- end

-- Single fighter

UM.Fighter = {}

function UM.Fighter:Source()
    return function (args)
        return args.fighter
    end
end

function UM.Fighter:Defender()
    return function (args)
        -- TODO
        return GetDefender()
    end
end

function UM.Fighter:Named(name)
    return UM.Select:Fighters()
        :Named(name)
        :BuildOne()
end

-- Single token

UM.Token = {}

function UM.Token:Source()
    return function (args)
        return args.token
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

-- Token creation

function UM.Build:Token()
    local result = {}

    result.amount = -1
    result.whenReturnedToBox = {}
    result.onStepEffects = {}

    function result:Build()
        return {
            Amount = result.amount,
            WhenReturnedToBox = result.whenReturnedToBox,
            OnStepEffects = result.onStepEffects,
        }
    end

    function result:Amount(v)
        result.amount = v

        return result
    end

    function result:WhenReturnedToBox(text, ...)
        result.whenReturnedToBox[#result.whenReturnedToBox+1] = {
            text = text,
            effects = {...}
        }
        return result
    end

    function result:OnStep(text, fighterPredFunc, ...)
        result.onStepEffects[#result.onStepEffects+1] = {
            text = text,
            fighterPred = fighterPredFunc,
            effects = {...}
        }
        return result
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
    result.onAttackEffects = {}
    result.afterAttackEffects = {}
    result.gameStartEffects = {}
    result.movementNodeConnections = {}
    result.cardCancellingForbids = {}
    result.onManoeuvreEffects = {}
    result.onDamageEffects = {}
    result.tokens = {}

    function result:ForbidCardCancelling(cardPredicate, byPlayerPredicate)
        result.cardCancellingForbids[#result.cardCancellingForbids+1] = function (args, card, player)
            if not cardPredicate(args, card) then
                return false
            end
            if not byPlayerPredicate(args, player) then
                return false
            end
            return true
        end

        return result
    end

    function result:DeclareToken(tokenName, tokenBehavior)
        result.tokens[tokenName] = tokenBehavior

        return result
    end

    function result:OnAttack(text, fighterPredFunc, ...)
        result.onAttackEffects[#result.onAttackEffects+1] = {
            text = text,
            fighterPred = fighterPredFunc,
            effects = {...}
        }
        return result
    end

    function result:AfterAttack(text, fighterPredFunc, ...)
        result.afterAttackEffects[#result.afterAttackEffects+1] = {
            text = text,
            fighterPred = fighterPredFunc,
            effects = {...}
        }
        return result
    end

    function result:ModManoeuvreValue(fighterPredFunc, modFunc)
        result.manoeuvreValueMods[#result.manoeuvreValueMods+1] = {
            fighterPred = fighterPredFunc,
            modFunc = modFunc,
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

    function result:ConnectNodesForMovement(fighterPred, fromNodePred, manyNodesTo)
        result.movementNodeConnections[#result.movementNodeConnections+1] = function (args, fighter, node)
            if not fighterPred(args, fighter) then
                return {}
            end
            if not fromNodePred(args, node) then
                return {}
            end
            local nodes = manyNodesTo(args)
            local resultNodes = {}
            for _, n in ipairs(nodes) do
                if n ~= node then
                    resultNodes[#resultNodes+1] = n
                end
            end
            return resultNodes
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

    function result:AtTheStartOfTheGame(text, ...)
        result.gameStartEffects[#result.gameStartEffects+1] = {
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

    function result:AtTheEndOfYourTurn(text, ...)
        return result:AddTurnPhaseEffects(UM.TurnPhaseTriggers.END, text, {...})
    end

    function result:OnManoeuvre(text, ...)
        result.onManoeuvreEffects[#result.onManoeuvreEffects+1] = {
            text = text,
            effects = {...}
        }
        return result
    end

    function result:OnDamage(text, ...)
        result.onDamageEffects[#result.onDamageEffects+1] = {
            text = text,
            effects = {...}
        }
        return result
    end

    function result:Build()
        local fighter = {
            TurnPhaseEffects = result.turnPhaseEffects,
            CardValueModifiers = result.cardValueModifiers,
            WhenPlacedEffects = result.whenPlaced,
            ManoeuvreValueMods = result.manoeuvreValueMods,
            OnAttackEffects = result.onAttackEffects,
            AfterAttackEffects = result.afterAttackEffects,
            Tokens = result.tokens,
            GameStartEffects = result.gameStartEffects,
            MovementNodeConnections = result.movementNodeConnections,
            CardCancellingForbids = result.cardCancellingForbids,
            OnManoeuvreEffects = result.onManoeuvreEffects,
            OnDamageEffects = result.onDamageEffects,
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

    function result:Last(args)
        return result.values[#result.values]
    end

    return result
    -- return function (...)
    --     return result
    -- end
end

function UM.Number:Count(many)
    local result = {}

    function result:Choose(args, hint)
        -- TODO
    end

    function result:Last(args)
        return #many(args)
    end

    return result
    -- return function (...)
    --     return result
    -- end
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
    return function (args)
        if not conditionalFunc(args) then
            falseEffectFunc(args)
            return
        end
        trueEffectFunc(args)
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

-- Conditions

UM.Conditions = {}

function UM.Conditions:FightersAreAdjacent(singleFighter1, singleFighter2)
    return function (args)
        local fighter1 = singleFighter1(args)
        if fighter1 == nil then
            return false
        end
        local fighter2 = singleFighter2(args)
        if fighter2 == nil then
            return false
        end
        return AreAdjacent(fighter1, fighter2)
    end
end

function UM.Conditions:FighterStandsOnNode(singleFighter, manyNodes)
    return function (args)
        local fighter = singleFighter(args)
        local nodes = manyNodes(args)
        for _, node in ipairs(nodes) do
            if node.Fighter == fighter then
                return true
            end
        end
        return false
    end
end

function UM.Conditions:PlayerAttributeEqualTo(attrKey, attrValue)
    return function (args)
        return GetPlayerStringAttribute(args.owner, attrKey) == attrValue
    end
end

function UM.Conditions:And(cond1, cond2)
    return function (args)
        return cond1(args) and cond2(args)
    end
end

function UM.Conditions:TokensLeft(tokenName)
    return function (args)
        return GetTokenAmount(tokenName) > 0
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

function UM.Conditions:CombatLostBy(playerFunc)
    return function (args)
        local combat = GetCombat()
        -- TODO assert that combat is not null
        -- TODO assert that combat winner is not null
        return combat.Winner ~= playerFunc(args)
    end
end

function UM.Conditions:CountGte(many, amount)
    return function (args)
        local objs = many(args)
        return #objs >= amount
    end
end

function UM.Conditions:Eq(numeric1, numeric2)
    return function (args)
        return numeric1:Last(args) == numeric2:Last(args)
    end
end

function UM.Conditions:Gt(numeric1, numeric2)
    return function (args)
        return numeric1:Last(args) > numeric2:Last(args)
    end
end

UM.Conditions.CharacterSpecific = {}

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

UM.Count.CharacterSpecific = {}

-- Card modifications

UM.Mod = {}
UM.Mod.Cards = {}

function UM.Mod.Cards:_(numeric, boostsAttackCards, boostsDefenseCards)
    return function (args, combatCard, result)
        local amount = numeric:Last(args)
        if not boostsAttackCards and not combatCard.IsDefence then
            return result
        end
        if not boostsDefenseCards and combatCard.IsDefence then
            return result
        end
        return result + amount
    end
end

function UM.Mod.Cards:AttackCards(numeric)
    return UM.Mod.Cards:_(numeric, true, false)
end

function UM.Mod.Cards:DefenseCards(numeric)
    return UM.Mod.Cards:_(numeric, false, true)
end

-- Effects

function UM.Effects:Draw(manyPlayers, numeric, optional)
    return function (args)
        local players = manyPlayers(args, 'Choose a player who will draw the cards')
        if optional then
            local choice = ChooseString(args.owner, {
                [1] = 'Yes',
                [2] = 'No'
            }, 'Draw card(s)?')
            if choice ~= 'Yes' then
                return
            end
        end
        for _, p in ipairs(players) do
            local amount = numeric:Choose(args, 'Choose how many cards to draw')
            DrawCards(p, amount)
        end
    end
end

function UM.Effects:MoveFighters(manyFighters, numeric, canMoveOverOpposing)
    return function (args)
        local fighters = manyFighters(args, 'Choose which fighter to move')
        local amount = numeric:Last(args)
        MoveFighters(args.owner, fighters, amount, canMoveOverOpposing) -- TODO this threw an exception after playing Skirmish and defeating a Harpy
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
        local players = manyPlayers(args, 'Choose which player will discard')
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
    return function (args)
        local f = true
        if optional then
            local choice = ChooseString(args.owner, {
                [1] = 'Yes',
                [2] = 'No'
            }, 'Boost your card?')
            if choice ~= 'Yes' then
                f = false
            end
        end

        if not f then
            return
        end

        local amount = numeric:Choose(args, 'Boost how many times?')
        local player = args.owner
        for i = 1, amount do
            if GetHandSize(player) == 0 then
                return
            end
            
            -- TODO check hand size
            local card = GetCardInHand(
                player,
                ChooseCardInHand(player, player, 'Choose a card for boost')
            )

            BoostCardInCombat(player, card)
        end
    end
end

function UM.Effects:DealDamage(manyFighters, numeric)
    return function (args)
        local fighters = manyFighters(args, 'Choose the target fighter for damage')

        for _, fighter in ipairs(fighters) do
            local amount = numeric:Choose(args, 'Choose how much damage to deal to '..fighter.Name)
            DealDamage(fighter, amount)
        end
    end
end

function UM.Effects:Recover(manyFighters, amount)
    return function (args)
        local fighters = manyFighters(args, 'Choose which fighter will recover health')

        for _, fighter in ipairs(fighters) do
            DealDamage(fighter, amount)
        end
    end
end

function UM.Effects:PlaceFighter(singleFighter, manySpaces)
    return function (args)
        local fighter = singleFighter(args, 'Choose which fighter to place')
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

function UM.Effects:CancelAllEffectsOnOpponentsCard()
    return function (args)
        local player = args.owner
        CancelCombatEffectsOfOpponent(player)
    end
end

function UM.Effects:CancelCurrentMovement()
    return function (args)
        CancelCurrentMovement()
    end
end

function UM.Effects:RemoveTokens(manyTokens)
    return function (args)
        local tokens = manyTokens(args, 'Choose which token to remove')
        for _, token in ipairs(tokens) do
            RemoveToken(token)
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

function UM.Effects:PlaceTokens(tokenName, manyNodes)
    return function (args)
        assert(IsTokenDefined(tokenName), 'Tried to place an undefined token: '..tokenName)

        local nodes = manyNodes(args, 'Choose where to place a '..tokenName..' token')

        for _, node in ipairs(nodes) do
            PlaceToken(node, tokenName)
        end
    end
end

function UM.Effects:MoveToken(singleToken, singleNode)
    return function (args)
        local token = singleToken(args, 'Choose what token to move')

        local node = singleNode(args, 'Choose where to move the '..token.Original.Name..' token')

        MoveToken(token, node)
    end
end


-- entity selectors

UM.Select = {}

function UM.Select:_Base(getAllFunc, chooseSingleFunc)
    local result = {}

    result.filters = {}
    result.single = false
    result.chooseHint = 'Choose'

    function result:_Add(filter)
        result.filters[#result.filters+1] = filter

        return result
    end

    function result:Single()
        result.single = true
        return result
    end

    function result:_Select(args)
        local all = getAllFunc()
        local objs = {}

        local filterFunc = function (obj)
            for _, filter in ipairs(result.filters) do
                if not filter(args, obj) then
                    return false
                end
            end
            return true
        end

        for _, obj in ipairs(all) do
            if filterFunc(obj) then
                objs[#objs+1] = obj
            end
        end

        -- TODO check for 0

        if result.single then
            local obj = objs[1]

            if #objs > 1 then
                obj = chooseSingleFunc(args.owner, objs, result.chooseHint)
            end

            objs = {
                [1] = obj
            }

        end

        return objs
    end

    function result:Build()
        return function (args, chooseHint)
            result.chooseHint = chooseHint or result.chooseHint
            return result:_Select(args)
        end
    end

    function result:BuildPredicate()
        return function (args, obj)
            local fighters = result:_Select(args)
            for _, v in ipairs(fighters) do
                if v == obj then
                    return true
                end
            end
            return false
        end
    end

    function result:BuildOne()
        result.single = true
        -- TODO? cache result
        return function (args, chooseHint)
            result.chooseHint = chooseHint or result.chooseHint
            local objs = result:_Select(args)
            -- TODO what if no objs
            return objs[1]
        end
    end

    return result
end

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
    local result = UM.Select:_Base(GetFighters, ChooseFighter)

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
        return result:_Add(function (args, fighter)
            -- TODO
            return args.fighter == fighter
        end)
    end

    function result:Named(...)
        local names = {...}
        result.filters[#result.filters+1] = function (args, fighter)
            for _, name in ipairs(names) do
                if IsCalled(fighter, name) then
                    return true
                end
            end
            return false
        end

        return result
    end

    function result:Except(singleFighter)
        return result:_Add(function (args, fighter)
            return fighter ~= singleFighter(args)
        end)
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

    function result:AdjacentTo(singleFighter)
        result.filters[#result.filters+1] = function (args, fighter)
            local f = singleFighter(args)
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

    function result:MovingFighter()
        return result:_Add(function (args, fighter)
            return IsMoving(fighter)
        end)
    end

    function result:OpposingTo(playerFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            return IsOpposingTo(fighter, playerFunc(args))
        end

        return result
    end

    function result:FriendlyTo(playerFunc)
        result.filters[#result.filters+1] = function (args, fighter)
            return not IsOpposingTo(fighter, playerFunc(args))
        end

        return result
    end

    function result:Opposing()
        return result:OpposingTo(UM.Player:EffectOwner())
    end

    function result:Friendly()
        return result:FriendlyTo(UM.Player:EffectOwner())
    end

    function result:YourFighter()
        -- TODO

        return result
    end

    return result
end

function UM.Select:Players()
    local result = UM.Select:_Base(GetPlayers, ChoosePlayer)

    result.filters = {}
    result.single = false

    -- function result:OpposingTo(playerFunc)
    --     -- TODO
    --     result.filters[#result.filters+1] = function (args, player)
    --         return AreOpposingPlayers(player, playerFunc(args))
    --     end
    --     return result
    -- end

    function result:You()
        return result:_Add(function (args, player)
            return args.owner == player
        end)
    end

    function result:Opponents()
        return result:_Add(function (args, player)
            return AreOpposingPlayers(args.owner, player)
        end)
    end

    function result:YourOpponent()
        -- TODO combat opponent

        return result
    end

    return result
end

function UM.Select:Nodes()
    local result = UM.Select:_Base(GetNodes, ChooseNode)

    function result:Unoccupied()
        return result:_Add(function (args, node)
            return IsUnoccupied(node)
        end)
    end

    function result:InZoneOfFighter(singleFighter)
        return result:_Add(function (args, node)
            return IsInZone(node, GetFighterZones(singleFighter(args)))
        end)
    end

    function result:WithNoToken(tokenName)
        return result:_Add(function (args, node)
            return not NodeContainsToken(node, tokenName)
        end)
    end

    function result:WithToken(tokenName)
        return result:_Add(function (args, node)
            return NodeContainsToken(node, tokenName)
        end)
    end

    function result:AdjacentToFighters(manyFighters)
        return result:_Add(function (args, node)
            local fighters = manyFighters(args)
            for _, fighter in ipairs(fighters) do
                local fighterNode = GetFighterNode(fighter)
                if AreNodesAdjacent(fighterNode, node) then
                    return true
                end
            end
            return false
        end)
    end

    return result
end

function UM.Select:Tokens()
    local result = UM.Select:_Base(GetTokens, ChooseToken)

    function result:Only(singleToken)
        return result:_Add(function (args, token)
            return token == singleToken(args)
        end)
    end

    function result:Named(name)
        return result:_Add(function (args, token)
            return token.Original.Name == name
        end)
    end

    return result
end

-- Character-specific

UM.Effects.CharacterSpecific = {}
