UM = {}

function UM:Card()
    local result = {}

    result.schemeEffects = {}

    function result:Effect(text, ...)
        result.schemeText = text
        local effects = {...}

        for _, v in ipairs(effects) do
            result.schemeEffects[#result.schemeEffects+1] = v
        end

        return result
    end

    function result:Build()
        return {
            SchemeEffects = result.schemeEffects
        }
    end

    return result
end

function UM:Static(amount)
    return function (...)
        return amount
    end
end

UM.Effects = {}

function UM.Effects:Draw(amountFunc)
    return function (fighter)
        local amount = amountFunc(fighter)
        print('Draw '..amount..' cards')
    end
end

function UM.Effects:GainActions(amountFunc)
    return function (fighter)
        local amount = amountFunc(fighter)
        print('Gain '..amount..' actions')
    end
end



-- -- Feint
-- function _Create()
--     return UM.Versatile('Feint', 2)
--         :Immediately(
--             UM.Effects:CancelAllEffectsOfOpponentCard(),
--             'Cancel all effects on your opponent\'s card.'
--         )
--         :Build()
-- end

-- -- Commanding Impact
-- function _Create()
--     return UM.Attack('Regroup', 5)
--         :AfterCombat(
--             UM.Effects:Draw(
--                 UM:Static(1),
--                 UM.Players:EffectOwner()
--             ),
--             'Draw 1 card'
--         )
--         :Build()
-- end

-- -- Piercing Shot
-- function _Create()
--     return UM.Attack('Piercing Shot', 2)
--         :AfterCombat(
--             UM.Effects:Draw(
--                 UM:Static(2),
--                 UM.Players:EffectOwner()
--             ),
--             'Draw 2 card'
--         )
--         :Build()
-- end

-- -- Willy Fighting
-- function _Create()
--     return UM.Versatile('Willy Fighting', 3)
--         :AfterCombat(
--             UM.Effects:DealDamage(
--                 UM:Static(1),
--                 UM.S.Fighters()
--                     :Opposing(UM.Players:EffectOwner())
--                     :Adjacent(UM.Fighters:Current())
--                     :Build()
--             ),
--             'Deal 1 damage to each opposing fighter adjacent to your fighter.'
--         )
--         :Build()
-- end

-- -- Swift strike
-- function _Create()
--     return UM.Attack('Swift Strike', 3)
--         :AfterCombat(
--             UM.Effects:MoveFighter(
--                 UM.UpTo(4),
--                 UM.Fighters:Current()
--             ),
--             'Move your fighter up to 4 spaces. '
--         )
--         :Build()
-- end

-- -- Snark
-- function _Create()
--     return UM.Versatile('Snark', 3)
--         :AfterCombat(
--             UM.If(
--                 UM.Conditional:FightersAdjacent(
--                     UM.Figthers():Current(),
--                     UM.Fighters():Opposing()
--                 ),
--                 UM.Effects.Draw(
--                     UM:Static(1)
--                     -- , UM.Players:EffectOwner() -- default is the effect owner
--                 )
--             ),
--             'If your fighter is adjacent to the opposing fighter, draw 1 card.'
--         )
--         :Build()
-- end

-- -- Rending Shot
-- function _Create()
--     return UM.Attack('Rending Shot', 3)
--         :AfterCombat(
--             UM.Effects:MoveFighter(
--                 UM.UpTo(3),
--                 UM.Fighters:Opposing()
--             ),
--             'Move the opposing fighter up to 3 spaces.'
--         )
--         :Build()
-- end

-- -- For My Next Trick
-- function _Create()
--     return UM.Attack('For My Next Trick', 2)
--         :AfterCombat(
--             UM.Effects:MoveFighter(
--                 UM.UpTo(1),
--                 UM.S.Fighters()
--                     :OwnedBy(UM.Players:EffectOwner())
--                     :Build()
--             ),
--             'Move one of your fighters up to 1 space.'
--         )
--         :AfterCombat(
--             UM.Effects.Draw(
--                 UM:Static(1)
--             ),
--             'Draw 1 card.'
--         )
--         :AfterCombat(
--             UM.Effects.GainAction(
--                 UM:Static(1)
--             ),
--             'Gain 1 action.'
--         )
--         :Build()
-- end

-- -- Evade
-- function _Create()
--     return UM.Defence('Evade', 3)
--         :AfterCombat(
--             UM.Effects.Draw(
--                 UM:Static(1)
--             ),
--             'Draw 1 card.'
--         )
--         :Build()
-- end

-- -- Closer Than She Appears
-- function _Create()
--     return UM.Scheme('Closer Than She Appears')
--         :Effect(
--             UM.Effects:MoveFighter(
--                 UM.UpTo(1),
--                 UM.S.Fighters()
--                     :OwnedBy(UM.Players:EffectOwner())
--                     :Build()
--             ),
--             'Move your fighter up to 1 space.'
--         )
--         :Effect(
--             UM.Effects.Draw(
--                 UM:Static(1)
--             ),
--             'Draw 1 card.'
--         )
--         :Effect(
--             UM.Effects.GainAction(
--                 UM:Static(1)
--             ),
--             'Gain 1 action.'
--         )
--         :Build()
-- end

-- -- Gaze of Stone
-- function _Create()
--     return UM.Attack('Gaze of Stone', 2)
--         :AfterCombat(
--             UM.If(
--                 UM.Conditional:CombatWonBy(
--                     UM.Players:EffectOwner()
--                 ),
--                 UM.Effects:DealDamage(
--                     UM:Static(8),
--                     UM.Figthers:Opposing()
--                 )
--             ),
--             'If you won the combat, deal 8 damage to the opposing fighter.'
--         )
--         :Build()
-- end

-- -- Gaze of Stone
-- function _Create()
--     return UM.Scheme('A Momentary Glance')
--         :Effect(
--             UM.Effects:DealDamage(
--                 UM:Static(2),
--                 UM.S.Fighters()
--                     .InZone:WithNamedFighter('Medusa')
--                     :Build()
--             ),
--             'Deal 2 damage to any one fighter in Medusa\'s zone.'
--         )
--         :Build()
-- end

-- -- Disarming Shot
-- function _Create()
--     return UM.Attack('Disarming Shot', 4)
--         :AfterCombat(
--             UM.Effects.Draw(
--                 UM:AmountOfDamageDealtTo(
--                     UM.Fighters:Opposing()
--                 )
--             ),
--             'Draw a number of cards equal to the amount of damage dealt to the opposing fighter.'
--         )
--         :Build()
-- end