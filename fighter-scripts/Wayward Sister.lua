-- Your cards go into your cauldron instead of your discard pile.
-- After you attack, you may cast one spell that you have the ingredients for. If you do, discard all the cards in your cauldron.


local function _defineSpell(name, spellEffect, ...)
    local ingredients = {...}
    local ingredientMap = {}
    for _, ingredient in ipairs(ingredients) do
        ingredientMap[ingredient] = 1
    end

    return {
        name = name,
        ingredients = ingredients,
        ingredientMap = ingredientMap,
        effectCollection = spellEffect
    }
end

function _Create()
    local spells = {
        -- snake + bat = Draw 1 card. One of your fighters recovers 1 health.
        _defineSpell(
            'Draw 1 card. One of your fighters recovers 1 health.',
            UM.Effects:Combine(
                UM.Effects:Draw(
                    UM.Select:Players():You():Build(),
                    UM.Number:Static(1),
                    false
                ),
                UM.Effects:Recover(
                    UM.Select:Fighters():AllYour():Single():Build(),
                    UM.Number:Static(1)
                )
            ),
            'snake', 'bat'
        ),
        -- snake + leg = Move any fighter up to 3 spaces.
        _defineSpell(
            'Move any fighter up to 3 spaces.',
            UM.Effects:MoveFighters(
                UM.Select:Fighters():Single():Build(),
                UM.Number:UpTo(3),
                false
            ),
            'snake', 'leg'
        ),
        -- leg + bat = Deal 1 damage to a fighter that shares a zone with any of your fighters.
        _defineSpell(
            'Deal 1 damage to a fighter that shares a zone with any of your fighters.',
            UM.Effects:DealDamage(
                UM.Select:Fighters()
                    :InSameZoneAsAny(
                        UM.Select:Fighters()
                            :AllYour()
                            :Build()
                    )
                    :Single()
                    :Build(),
                UM.Number:Static(1)
            ),
            'leg', 'bat'
        ),
        -- snake + leg + bat = Gain 1 action.
        _defineSpell(
            'Gain 1 action',
            UM.Effects:GainActions(1),
            'snake', 'leg', 'bat'
        )
    }
    return UM.Build:Fighter()
        :DefineCardZone(
            'CAULDRON',
            UM.Build:CardZone()
                :Build()
        )
        :RedirectCardZoneChange(
            -- 'WAYWARD_SISTERS_CARD_REDIRECTOR', -- id, to prevent multiple card zone change redirectors
            'Your cards go into your cauldron instead of your discard pile.',
            function (args, zoneChange)
                local card = zoneChange.Card
                -- if isn't card owner
                if card.Owner ~= args.owner then
                    return false
                end
                -- if isn't going to discard
                local targetZone = zoneChange.TargetZone
                if targetZone:GetName() ~= 'DISCARD' then
                    return false
                end

                -- check that is not discarding a card from the cauldron
                local fromZone = zoneChange.FromZone
                if fromZone:GetName() == 'CAULDRON' then
                    return false
                end

                local zone = GetZone(args.owner, 'CAULDRON')
                ChangeTargetZone(zoneChange, zone)
                return true
            end
        )
        :AfterAttack(
            'After you attack, you may cast one spell that you have the ingredients for. If you do, discard all the cards in your cauldron.',
            UM.Select:Fighters():Source():BuildPredicate(),
            UM.Effects.CharacterSpecific:PromptCastCauldronSpell(spells, true)
        )
    :Build()
end

function UM.Effects.CharacterSpecific:PromptCastCauldronSpell(spells, clearCauldron)

    local canCastSpell = function (spell, ingredientMap)
        for ingredient, requirement in pairs(spell.ingredientMap) do
            if ingredientMap[ingredient] < requirement then
                return false
            end
        end
        return true
    end

    local getCastableSpells = function (args)
        local ingredientMap = {
            snake = 0,
            leg = 0,
            bat = 0,
        }
        local cards = GetCardsInZone(args.owner, 'CAULDRON')
        for _, card in ipairs(cards) do
            for ingredient, v in pairs(ingredientMap) do
                if CardHasLabel(card, ingredient) then
                    ingredientMap[ingredient] = v + 1
                    break
                end
            end
        end

        local result = {}
        for _, spell in ipairs(spells) do
            if canCastSpell(spell, ingredientMap) then
                result[#result+1] = spell
            end
        end

        return result
    end

    return function (args)
        local castableSpells = getCastableSpells(args)
        if #castableSpells == 0 then
            return
        end

        local options = {
            [1] = 'Nothing'
        }
        local spellMap = {}
        for _, spell in ipairs(castableSpells) do
            options[#options+1] = spell.name
            spellMap[spell.name] = spell
        end

        local choice = ChooseString(args.owner, options, 'Cast which spell?')
        if choice == 'Nothing' then
            return
        end

        local spell = spellMap[choice]
        spell.effectCollection(args)

        if not clearCauldron then
            return
        end

        UM.Effects.CharacterSpecific:ClearCauldron()(args)
    end
end

function UM.Effects.CharacterSpecific:ClearCauldron()
    return function (args)
        MoveAllCards(args.owner, 'CAULDRON', 'DISCARD')
    end
end