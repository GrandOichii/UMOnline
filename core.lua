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

function UM.Player:Current()
    return function (args)
        return GetCurrentPlayer()
    end
end

function UM.Player:Opponent()
    return UM.Select:Players()
        :YourOpponent()
        :BuildOne()
end

-- Single fighter

UM.Fighter = {}

function UM.Fighter:Source()
    return function (args)
        return args.fighter
    end
end

function UM.Fighter:Defender()
    return function (args)
        return GetDefender()
    end
end

function UM.Fighter:Attacker()
    return function (args)
        return GetAttacker()
    end
end

function UM.Fighter:Moving()
    return function (args)
        return GetMovingFighter()
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

-- Builders

UM.Build = {}

-- Effect collection

function UM.Build:EffectCollection()
    local effectCollection = {}

    effectCollection.text = ''
    effectCollection.effects = {}
    effectCollection.conds = {}

    function effectCollection:Build()
        return {
            text = effectCollection.text,
            effects = effectCollection.effects,
            cond = function (args, subjects)
                for _, check in ipairs(effectCollection.conds) do
                    if not check(args, subjects) then
                        return false
                    end
                end
                return true
            end
        }
    end

    function effectCollection:AddCond(cond)
        effectCollection.conds[#effectCollection.conds+1] = cond

        return effectCollection
    end

    function effectCollection:SourceIsAlive()
        return effectCollection:AddCond(function (args)
            return IsAlive(args.fighter)
        end)
    end

    function effectCollection:Text(text)
        effectCollection.text = text

        return effectCollection
    end

    function effectCollection:Effects(effects)
        effectCollection.effects = effects

        return effectCollection
    end

    return effectCollection
end

-- Card creation

function UM.Build:_WithCombatSteps()
    local combatStepContainer = {}

    combatStepContainer.combatStepEffects = {}

    function combatStepContainer:CombatStepEffect(step, text, ...)
        combatStepContainer.combatStepEffects[step] = UM.Build:EffectCollection()
            :Text(text)
            :SourceIsAlive()
            :Effects({...})
            :Build()

        return combatStepContainer
    end

    function combatStepContainer:Immediately(text, ...)
        return combatStepContainer:CombatStepEffect(UM.CombatSteps.IMMEDIATELY, text, ...)
    end

    function combatStepContainer:DuringCombat(text, ...)
        return combatStepContainer:CombatStepEffect(UM.CombatSteps.DURING_COMBAT, text, ...)
    end

    function combatStepContainer:AfterCombat(text, ...)
        return combatStepContainer:CombatStepEffect(UM.CombatSteps.AFTER_COMBAT, text, ...)
    end

    return combatStepContainer
end

function UM.Build:Card()
    local card = UM.Build:_WithCombatSteps()

    card.scheme = UM.Build:EffectCollection()
        :SourceIsAlive()

    card.schemeRequirements = {}

    function card:Effect(text, ...)
        card.scheme = card.scheme
            :Text(text)
            :Effects({...})

        return card
    end

    function card:SchemeRequirement(text, condition)
        card.schemeRequirements[#card.schemeRequirements+1] = {
            text = text,
            checkFunc = condition,
        }

        return card
    end

    function card:Build()
        return {
            Scheme = card.scheme:Build(),
            SchemeRequirements = card.schemeRequirements,
            CombatStepEffects = card.combatStepEffects,
        }
    end

    return card
end

-- Token creation

function UM.Build:Token()
    local token = {}

    token.amount = -1
    token.whenReturnedToBox = {}
    token.onStepEffects = {}

    function token:Build()
        return {
            Amount = token.amount,
            WhenReturnedToBox = token.whenReturnedToBox,
            OnStepEffects = token.onStepEffects,
        }
    end

    function token:Amount(v)
        token.amount = v

        return token
    end

    function token:WhenReturnedToBox(text, ...)
        token.whenReturnedToBox[#token.whenReturnedToBox+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :Build()

        return token
    end

    function token:OnStep(text, fighterPredFunc, ...)
        token.onStepEffects[#token.onStepEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredFunc)
            :Build()

        return token
    end

    return token
end

-- Fighter creation

function UM.Build:Fighter()
    local fighter = UM.Build:_WithCombatSteps()

    fighter.turnPhaseEffects = {}
    fighter.cardValueModifiers = {}
    fighter.whenPlaced = {}
    fighter.manoeuvreValueMods = {}
    fighter.onAttackEffects = {}
    fighter.afterAttackEffects = {}
    fighter.afterSchemeEffects = {}
    fighter.gameStartEffects = {}
    fighter.movementNodeConnections = {}
    fighter.cardCancellingForbids = {}
    fighter.onManoeuvreEffects = {}
    fighter.onDamageEffects = {}
    fighter.onFighterDefeatEffects = {}
    fighter.damageModifiers = {}
    fighter.afterMovementEffects = {}
    fighter.boostedMovementReplacers = {}
    fighter.onMoveEffects = {}
    fighter.manoeuvreDrawAmountModifiers = {}
    fighter.onLostCombatEffects = {}
    fighter.onCombatCardChoiceEffects = {}
    fighter.whenManoeuvreEffects = {}
    fighter.tokens = {}

    function fighter:Build()
        local result = {
            TurnPhaseEffects = fighter.turnPhaseEffects,
            CardValueModifiers = fighter.cardValueModifiers,
            WhenPlacedEffects = fighter.whenPlaced,
            ManoeuvreValueMods = fighter.manoeuvreValueMods,
            OnAttackEffects = fighter.onAttackEffects,
            AfterAttackEffects = fighter.afterAttackEffects,
            AfterSchemeEffects = fighter.afterSchemeEffects,
            Tokens = fighter.tokens,
            GameStartEffects = fighter.gameStartEffects,
            MovementNodeConnections = fighter.movementNodeConnections,
            CardCancellingForbids = fighter.cardCancellingForbids,
            OnManoeuvreEffects = fighter.onManoeuvreEffects,
            OnDamageEffects = fighter.onDamageEffects,
            OnFighterDefeatEffects = fighter.onFighterDefeatEffects,
            CombatStepEffects = fighter.combatStepEffects,
            DamageModifiers = fighter.damageModifiers,
            AfterMovementEffects = fighter.afterMovementEffects,
            BoostedMovementReplacers = fighter.boostedMovementReplacers,
            ManoeuvreDrawAmountModifiers = fighter.manoeuvreDrawAmountModifiers,
            OnLostCombatEffects = fighter.onLostCombatEffects,
            OnCombatCardChoiceEffects = fighter.onCombatCardChoiceEffects,
            WhenManoeuvreEffects = fighter.whenManoeuvreEffects,
            OnMoveEffects = fighter.onMoveEffects,
        }
        return result
    end

    function fighter:WhenManoeuvre(text, fighterPredicate, ...)
        fighter.whenManoeuvreEffects[#fighter.whenManoeuvreEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredicate)
            :Build()

        return fighter
    end

    function fighter:OnCombatCardChoice(text, ...)
        fighter.onCombatCardChoiceEffects[#fighter.onCombatCardChoiceEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :Build()

        return fighter
    end

    function fighter:OnLostCombat(text, ...)
        fighter.onLostCombatEffects[#fighter.onLostCombatEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :Build()

        return fighter
    end

    function fighter:ModManoeuvreCardDraw(modFunc)
        fighter.manoeuvreDrawAmountModifiers[#fighter.manoeuvreDrawAmountModifiers+1] = modFunc

        return fighter
    end

    function fighter:OnMove(text, effectFunc)
        fighter.onMoveEffects[#fighter.onMoveEffects+1] = {
            -- cond = isAliveCond(),
            text = text,
            effect = effectFunc,
        }

        return fighter
    end

    function fighter:ReplaceBoostedMovement(replacerFunc)
        fighter.boostedMovementReplacers[#fighter.boostedMovementReplacers+1] = replacerFunc

        return fighter
    end

    function fighter:AfterMove(text, fighterPredFunc, ...)
        fighter.afterMovementEffects[#fighter.afterMovementEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredFunc)
            :Build()

        return fighter
    end

    function fighter:OnFighterDefeat(text, fighterPredFunc, ...)
        fighter.onFighterDefeatEffects[#fighter.onFighterDefeatEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredFunc)
            :Build()

        return fighter
    end

    function fighter:OnFighterDefeatUngated(text, fighterPredFunc, ...)
        fighter.onFighterDefeatEffects[#fighter.onFighterDefeatEffects+1] = UM.Build:EffectCollection()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredFunc)
            :Build()

        return fighter
    end

    function fighter:ModifyDamage(damageModFunc)
        fighter.damageModifiers[#fighter.damageModifiers+1] = damageModFunc

        return fighter
    end

    function fighter:ForbidCardCancelling(cardPredicate, byPlayerPredicate)
        fighter.cardCancellingForbids[#fighter.cardCancellingForbids+1] = function (args, card, player)
            if not cardPredicate(args, card) then
                return false
            end
            if not byPlayerPredicate(args, player) then
                return false
            end
            return true
        end

        return fighter
    end

    function fighter:DeclareToken(tokenName, tokenBehavior)
        fighter.tokens[tokenName] = tokenBehavior

        return fighter
    end

    function fighter:OnAttack(text, fighterPredFunc, ...)
        fighter.onAttackEffects[#fighter.onAttackEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredFunc)
            :Build()

        return fighter
    end

    function fighter:AfterAttack(text, fighterPredFunc, ...)
        fighter.afterAttackEffects[#fighter.afterAttackEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredFunc)
            :Build()

        return fighter
    end

    function fighter:AfterScheme(text, fighterPredFunc, ...)
        fighter.afterSchemeEffects[#fighter.afterSchemeEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPredFunc)
            :Build()
            
        return fighter
    end

    function fighter:ModManoeuvreValue(fighterPredFunc, modFunc)
        fighter.manoeuvreValueMods[#fighter.manoeuvreValueMods+1] = {
            fighterPred = fighterPredFunc,
            modFunc = modFunc,
        }
        return fighter
    end

    function fighter:ModCardValue(text, fighterPredFunc, modFunc, modCondition)
        fighter.cardValueModifiers[#fighter.cardValueModifiers+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({
                [1] = modFunc
            })
            :AddCond(fighterPredFunc)
            :AddCond(modCondition)
            :Build()

        return fighter
    end

    function fighter:ConnectNodesForMovement(fighterPred, fromNodePred, manyNodesTo)
        fighter.movementNodeConnections[#fighter.movementNodeConnections+1] = function (args, fighter, node)
            if not fighterPred(args, fighter) then
                return {}
            end
            if not fromNodePred(args, node) then
                return {}
            end
            local nodes = manyNodesTo(args)
            local result = {}
            for _, n in ipairs(nodes) do
                if n ~= node then
                    result[#result+1] = n
                end
            end
            return result
        end

        return fighter
    end

    function fighter:WhenPlaced(text, ...)
        fighter.whenPlaced[#fighter.whenPlaced+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :Build()
        return fighter
    end

    function fighter:AtTheStartOfTheGame(text, ...)
        fighter.gameStartEffects[#fighter.gameStartEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :Build()

        return fighter
    end

    function fighter:AddTurnPhaseEffects(step, text, effects)
        fighter.turnPhaseEffects[step] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects(effects)
            :Build()

        return fighter
    end

    function fighter:AtTheStartOfYourTurn(text, ...)
        return fighter:AddTurnPhaseEffects(UM.TurnPhaseTriggers.START, text, {...})
    end

    function fighter:AtTheEndOfYourTurn(text, ...)
        return fighter:AddTurnPhaseEffects(UM.TurnPhaseTriggers.END, text, {...})
    end

    function fighter:OnManoeuvre(text, fighterPred, ...)
        fighter.onManoeuvreEffects[#fighter.onManoeuvreEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :Text(text)
            :Effects({...})
            :AddCond(fighterPred)
            :Build()

        return fighter
    end

    -- TODO add fighter predicate
    function fighter:OnDamage(text, fighterPred, ...)
        fighter.onDamageEffects[#fighter.onDamageEffects+1] = UM.Build:EffectCollection()
            :SourceIsAlive()
            :AddCond(fighterPred)
            :Text(text)
            :Effects({...})
            :Build()

        return fighter
    end

    return fighter
end

-- Combat

UM.Combat = {}

function UM.Combat:DamageDealt()
    return UM.Number:_(function (args)
        local combat = GetCombat()
        assert(combat ~= nil, 'TODO write this error')

        local damage = combat.DamageDealt
        if damage == nil then
            return {
                [1] = 0
            }
        end

        return {
            [1] = damage
        }
    end)
end

-- numeric

UM.Number = {}

function UM.Number:_(optionsFunc)
    local number = {}

    function number:Choose(args, hint)
        local values = optionsFunc(args)
        local result = values[1]
        if #values > 1 then
            result = ChooseNumber(args.owner, values, hint)
        end
        return result
    end

    function number:Last(args)
        local values = optionsFunc(args)
        return values[#values]
    end

    return number
end

function UM.Number:Count(many)
    return UM.Number:_(function (args)
        return {
            [1] = #many(args)
        }
    end)
end

function UM.Number:Static(v)
    return UM.Number:_(function (args)
        return {
            [1] = v
        }
    end)
end

function UM.Number:UpTo(max)
    local values = {}
    for i = 1, max do
        values[#values+1] = i
    end
    return UM.Number:_(function (args)
        return values
    end)
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

function UM.Effects:If(conditionalFunc, ...)
    local effects = {...}
    return function (args)
        if not conditionalFunc(args) then
            return
        end
        for _, e in ipairs(effects) do
            e(args)
        end
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

-- Actions

UM.Actions = {
    MANOEUVRE = 'Manoeuvre'
}

-- Comparison operators

UM.CmpOp = {}

function UM.CmpOp:Lt()
    return function (v1, v2)
        return v1 < v2
    end
end

function UM.CmpOp:Gt()
    return function (v1, v2)
        return v1 > v2
    end
end

-- Conditions

UM.Conditions = {}

function UM.Conditions:True()
    return function (args)
        return true
    end
end

function UM.Conditions:False()
    return function (args)
        return false
    end
end

function UM.Conditions:Not(cond)
    return function (args)
        return not cond(args)
    end
end

function UM.Conditions:IsAttacker(singlePlayer)
    return function (args)
        local part = GetCombatPart(singlePlayer(args))
        return part[1] ~= nil and not part[1].IsDefence
    end
end

function UM.Conditions:IsAttackingFighter(singleFighter)
    return function (args)
        local attacker = UM.Fighter:Attacker()(args)
        return singleFighter(args) == attacker
    end
end

function UM.Conditions:IsDefender(singlePlayer)
    return function (args)
        local part = GetCombatPart(singlePlayer(args))
        return part[3]
    end
end

function UM.Conditions:PerformedActionThisTurn(action)
    return function (args)
        local player = args.owner
        return PerformedActionThisTurn(player, action)
    end
end

function UM.Conditions:FighterAttackedThisTurn(singleFighter)
    return function (args)
        local fighter = singleFighter(args)
        return FighterAttackedThisTurn(fighter)
    end
end

function UM.Conditions:CmpHealth(singleFighter1, singleFighter2, cmpOp)
    return function (args)
        -- TODO what if fighters are nil
        local fighter1 = singleFighter1(args)
        local fighter2 = singleFighter2(args)

        return cmpOp(GetHealth(fighter1), GetHealth(fighter2))
    end
end

function UM.Conditions:CmpCombatPrintedValue(singlePlayer1, singlePlayer2, cmpOp)
    return function (args)
        -- TODO what if fighters are nil
        local p1 = singlePlayer1(args)
        local p2 = singlePlayer2(args)

        local p1Value = GetCombatPart(p1)[1].Value
        local p2Value = GetCombatPart(p2)[1].Value

        return cmpOp(p1Value, p2Value)
    end
end

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
        if fighter1 == fighter2 then
            return false
        end
        return AreAdjacent(fighter1, fighter2)
    end
end

function UM.Conditions:PlayedCombatCard(singlePlayer)
    return function (args)
        local part = GetCombatPart(singlePlayer(args))
        return part[1] ~= nil
    end
end

-- function UM.Conditions:AttackerOwnedBy(singlePlayer)
--     return function (args)
--         local combat = GetCombat()
--         assert(combat ~= nil, 'TODO write this error')

--         return combat.Attacker.Owner == singlePlayer(args)
--     end
-- end

-- function UM.Conditions:AttackerIs(singleFighter)
--     return function (args)
--         local combat = GetCombat()
--         assert(combat ~= nil, 'TODO write this error')

--         return combat.Attacker == singleFighter(args)
--     end
-- end

function UM.Conditions:FightersAreDefeated(manyFighters)
    return function (args)
        local fighters = manyFighters(args)
        for _, f in ipairs(fighters) do
            if not IsDefeated(f) then
                return false
            end
        end
        return true
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

function UM.Conditions:And(...)
    local conds = {...}
    return function (args)
        for _, cond in ipairs(conds) do
            if not cond(args) then
                return false
            end
        end
        return true
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

function UM.Conditions:CountEq(many, amount)
    return function (args)
        local objs = many(args)
        return #objs == amount
    end
end

function UM.Conditions:Eq(numeric1, numeric2)
    return function (args)
        LogPublic(tostring(numeric1:Last(args))..' '..tostring(numeric2:Last(args)))
        return numeric1:Last(args) == numeric2:Last(args)
    end
end

function UM.Conditions:Gt(numeric1, numeric2)
    return function (args)
        return numeric1:Last(args) > numeric2:Last(args)
    end
end

function UM.Conditions:Gte(numeric1, numeric2)
    return function (args)
        return numeric1:Last(args) >= numeric2:Last(args)
    end
end

function UM.Conditions:Lte(numeric1, numeric2)
    return function (args)
        return numeric1:Last(args) <= numeric2:Last(args)
    end
end

function UM.Conditions:Lt(numeric1, numeric2)
    return function (args)
        return numeric1:Last(args) < numeric2:Last(args)
    end
end

UM.Conditions.CharacterSpecific = {}

-- Counters

UM.Count = {}

function UM.Count:CardsInHand(singlePlayer)
    return UM.Number:_(function (args)
        return {
            [1] = GetHandSize(singlePlayer(args))
        }
    end)
end

UM.Count.CharacterSpecific = {}

-- Card modifications

UM.Mod = {}
UM.Mod.Cards = {}

function UM.Mod.Cards:_(numeric, boostsAttackCards, boostsDefenseCards)
    return function (args, subjects, result)
        local amount = numeric:Last(args)
        if not boostsAttackCards and not subjects.combatPart.IsDefence then
            return result
        end
        if not boostsDefenseCards and subjects.combatPart.IsDefence then
            return result
        end
        return result + amount
    end
end

function UM.Mod.Cards:AttackCards(numeric)
    return UM.Mod.Cards:_(numeric, true, false)
end

function UM.Mod.Cards:AllCards(numeric)
    return UM.Mod.Cards:_(numeric, true, true)
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

function UM.Effects:DrawTo(manyPlayers, numeric)
    -- TODO this can kill
    local drawUntilNCardsInHand = function (player, amount)
        while GetHandSize(player) < amount and not IsWinnerDetermined() do
            DrawCards(player, 1)
        end
    end

    return function (args)
        local players = manyPlayers(args, 'Choose a player who will draw the cards')
        for _, p in ipairs(players) do
            local amount = numeric:Choose(args, 'Draw until how many cards in hand?')
            if GetHandSize(p) < amount then
                drawUntilNCardsInHand(p, amount)
            end
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

function UM.Effects:Discard(manyPlayers, fixedNumber, random, ctxKey)
    local discardCards = function (player, amount, cardIdxFunc)
        local result = {}
        while amount > 0 do
            local idx = cardIdxFunc()
            local discarded = DiscardCard(player, idx)
            amount = amount - 1
            result[#result+1] = discarded
        end
        return result
    end

    return function (args)
        local players = manyPlayers(args, 'Choose which player will discard')
        local amount = fixedNumber

        for _, player in ipairs(players) do
            local discarded
            if random then
                discarded = discardCards(player, amount, function ()
                    return Rnd(GetHandSize(player))
                end)
            else
                discarded = discardCards(player, amount, function ()
                    return ChooseCardInHand(player, player, 'Choose a card to discard')
                end)
            end
            if ctxKey == nil then
                return
            end
            args.ctx[ctxKey] = discarded
        end
    end

end

function UM.Effects:BlindBoost(numeric, manyPlayers)
    local blindBoost = function (player)
        local milled = Mill(player, 1)
        if #milled == 0 then
            return
        end

        local card = milled[1]
        local boost = GetBoostValue(card)
        if boost == nil then
            return
        end

        AddToCardValueInCombat(player, boost)
    end

    return function (args)
        local players = manyPlayers(args, 'BLIND BOOST whose card?')
        local number = numeric:Choose(args, 'BLIND BOOST how many times?')

        for _, player in ipairs(players) do
            for i = 1, number do
                blindBoost(player)
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

function UM.Effects:ReviveAndSummon(singleDefeatedFighter, singleNode)
    return function (args)
        local fighter = singleDefeatedFighter(args, 'Choose which fighter to revive')
        assert(IsDefeated(fighter), 'Provided a non-defeated fighter for UM.Effects:ReviveAndSummon')
        FullyRecoverHealth(fighter)

        local node = singleNode(args, 'Choose where to place the fighter')
        PlaceFighter(fighter, node)
    end
end

function UM.Effects:Recover(manyFighters, number)
    return function (args)
        local fighters = manyFighters(args, 'Choose which fighter will recover health')
        local amount = number:Choose(args, 'Choose how much health to recover')

        for _, fighter in ipairs(fighters) do
            RecoverHealth(fighter, amount)
        end
    end
end

function UM.Effects:RevealTopCardOfDeck(manyPlayers, ctxKey)
    return function (args)
        local players = manyPlayers(args, 'Choose who will reveal the top card of their deck')
        local result = {}
        for _, player in ipairs(players) do
            local deck = GetDeck(player)
            if #deck > 0 then
                local card = deck[1]
                result[#result+1] = card
                LogPublic('Top card of player '..player.FormattedLogName..' is '..card.FormattedLogName)
            end
        end
        if ctxKey == nil then
            return
        end
        args.ctx[ctxKey] = result
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

function UM.Select:_Base(subjectKey, getAllFunc, chooseSingleFunc)
    local selector = {}

    selector.filters = {}
    selector.single = false
    selector.chooseHint = 'Choose'

    function selector:_Add(filter)
        selector.filters[#selector.filters+1] = filter

        return selector
    end

    function selector:Single()
        selector.single = true
        return selector
    end

    function selector:_Select(args)
        local all = getAllFunc()
        local objs = {}

        local filterFunc = function (obj)
            for _, filter in ipairs(selector.filters) do
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

        if selector.single then
            local obj = objs[1]

            if #objs > 1 then
                obj = chooseSingleFunc(args.owner, objs, selector.chooseHint)
            end

            objs = {
                [1] = obj
            }

        end

        return objs
    end

    function selector:Build()
        return function (args, chooseHint)
            selector.chooseHint = chooseHint or selector.chooseHint
            return selector:_Select(args)
        end
    end

    function selector:BuildFirst()
        return function (args, chooseHint)
            local results = selector:_Select(args)
            return results[1]
        end
    end

    function selector:BuildPredicate()
        return function (args, subjects)
            local fighters = selector:_Select(args)
            for _, v in ipairs(fighters) do
                if v == subjects[subjectKey] then
                    return true
                end
            end
            return false
        end
    end

    function selector:BuildContains()
        return function (args, obj)
            local fighters = selector:_Select(args)
            for _, v in ipairs(fighters) do
                if v == obj then
                    return true
                end
            end
            return false
        end
    end

    function selector:BuildOne()
        selector.single = true
        -- TODO? cache selector
        return function (args, chooseHint)
            selector.chooseHint = chooseHint or selector.chooseHint
            local objs = selector:_Select(args)
            -- TODO what if no objs
            return objs[1]
        end
    end

    return selector
end

function UM.Select:CardsInDiscardPile(ofPlayer)
    local selector = {}

    selector.filters = {}

    function selector:Count()
        return function (args)
            return #selector:_Select(args)
        end
    end

    function selector:_Select(args)
        local player = ofPlayer(args)
        local allCards = GetCardsInDiscardPile(player)
        local cards = {}

        local filterFunc = function (card)
            for _, filter in ipairs(selector.filters) do
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

        -- if selector.single then
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

    function selector:_Add(func)
        selector.filters[#selector.filters+1] = func

        return selector
    end

    function selector:WithLabel(label)
        return selector:_Add(function (args, card)
            return CardHasLabel(card, label)
        end)
    end

    function selector:Build()
        return function (args)
            return selector:_Select(args)
        end
    end

    return selector
end

function UM.Select:Fighters()
    local selector = UM.Select:_Base('fighter', GetFighters, ChooseFighter)

    function selector:OwnedBy(playerFunc)
        return selector:_Add(function (args, fighter)
            return fighter.Owner.Idx == playerFunc(args).Idx
        end)
    end

    function selector:OtherThanSource()
        return selector:Except(UM.Fighter:Source())
    end

    function selector:AllYour()
        return selector:OwnedBy(UM.Player:EffectOwner())
    end

    function selector:Undefeated()
        return selector:_Add(function (args, fighter)
            return not IsDefeated(fighter)
        end)
    end

    function selector:Defeated()
        return selector:_Add(function (args, fighter)
            return IsDefeated(fighter)
        end)
    end

    function selector:Your()
        return selector:_Add(function (args, fighter)
            return args.fighter == fighter
        end)
    end

    function selector:YourFighter()
        return selector:Your()
    end

    function selector:Named(...)
        local names = {...}

        return selector:_Add(function (args, fighter)
            for _, name in ipairs(names) do
                if IsCalled(fighter, name) then
                    return true
                end
            end
            return false
        end)
    end

    function selector:Except(singleFighter)
        return selector:_Add(function (args, fighter)
            return fighter ~= singleFighter(args)
        end)
    end

    function selector:Source()
        return selector:_Add(function (args, fighter)
            return fighter == args.fighter
        end)
    end

    function selector:InCombat()
        return selector:_Add(function (args, fighter)
            return IsInCombat(fighter)
        end)
    end

    function selector:Only(fighterFunc)
        return selector:_Add(function (args, fighter)
            local f = fighterFunc(args)
            -- if not IsAlive(f) then
            --     return false
            -- end
            return fighter == f
        end)
    end

    function selector:AdjacentTo(singleFighter)
        return selector:_Add(function (args, fighter)
            local f = singleFighter(args)
            if not IsAlive(f) then
                return false
            end
            return AreAdjacent(fighter, f)
        end)
    end

    function selector:InSameZoneAs(singleFighter)
        return selector:_Add(function (args, fighter)
            local f = singleFighter(args)
            if not IsAlive(f) then
                return false
            end
            return AreInSameZone(fighter, f)
        end)
    end

    function selector:OpposingInCombatTo(fighterFunc)
        return selector:_Add(function (args, fighter)
            local f = fighterFunc(args)
            if not IsAlive(f) then
                return false
            end
        end)
    end

    function selector:MovingFighter()
        return selector:_Add(function (args, fighter)
            return IsMoving(fighter)
        end)
    end

    function selector:OpposingTo(playerFunc)
        return selector:_Add(function (args, fighter)
            return IsOpposingTo(fighter, playerFunc(args))
        end)
    end

    function selector:FriendlyTo(playerFunc)
        return selector:_Add(function (args, fighter)
            return not IsOpposingTo(fighter, playerFunc(args))
        end)
    end

    function selector:MovedThrough()
        return selector:_Add(function (args, fighter)
            local fighters = GetMovedThroughFighters()
            for _, f in ipairs(fighters) do
                if f == fighter then
                    return true
                end
            end
            return false
        end)
    end

    function selector:Opposing()
        return selector:OpposingTo(UM.Player:EffectOwner())
    end

    function selector:Friendly()
        return selector:FriendlyTo(UM.Player:EffectOwner())
    end

    return selector
end

function UM.Select:Players()
    local selector = UM.Select:_Base('player', GetPlayers, ChoosePlayer)

    selector.filters = {}
    selector.single = false

    -- function selector:OpposingTo(playerFunc)
    --     -- TODO
    --     selector.filters[#selector.filters+1] = function (args, player)
    --         return AreOpposingPlayers(player, playerFunc(args))
    --     end
    --     return selector
    -- end

    function selector:You()
        return selector:_Add(function (args, player)
            return args.owner == player
        end)
    end

    function selector:Opponents()
        return selector:_Add(function (args, player)
            return AreOpposingPlayers(args.owner, player)
        end)
    end

    function selector:YourOpponent()
        return selector:_Add(function (args, player)
            local owner = args.owner
            return GetOpponentOf(owner) == player
        end)
    end

    function selector:InCombat()
        return selector:_Add(function (args, player)
            local part = GetCombatPart(player)[1]
            return part ~= nil
        end)
    end

    return selector
end

function UM.Select:Nodes()
    local selector = UM.Select:_Base('node', GetNodes, ChooseNode)

    function selector:Unoccupied()
        return selector:_Add(function (args, node)
            return IsUnoccupied(node)
        end)
    end

    function selector:NotInZone(zone)
        return selector:_Add(function (args, node)
            return not IsInZone(node, { [1] = zone })
        end)
    end

    function selector:WithFighter(singleFighter)
        return selector:_Add(function (args, node)
            local fighter = singleFighter(args)
            return node.Fighter == fighter
        end)
    end

    function selector:Empty()
        return selector:_Add(function (args, node)
            return IsNodeEmpty(node)
        end)
    end

    function selector:InZoneOfFighter(singleFighter)
        return selector:_Add(function (args, node)
            return IsInZone(node, GetFighterZones(singleFighter(args)))
        end)
    end

    function selector:WithNoToken(tokenName)
        return selector:_Add(function (args, node)
            return not NodeContainsToken(node, tokenName)
        end)
    end

    function selector:WithToken(tokenName)
        return selector:_Add(function (args, node)
            return NodeContainsToken(node, tokenName)
        end)
    end

    function selector:AdjacentToFighters(manyFighters)
        return selector:_Add(function (args, node)
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

    return selector
end

function UM.Select:Tokens()
    local selector = UM.Select:_Base('token', GetTokens, ChooseToken)

    function selector:Only(singleToken)
        return selector:_Add(function (args, token)
            return token == singleToken(args)
        end)
    end

    function selector:Named(name)
        return selector:_Add(function (args, token)
            return token.Original.Name == name
        end)
    end

    return selector
end

-- Character-specific

UM.Effects.CharacterSpecific = {}