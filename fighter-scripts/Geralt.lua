-- 

function _Create()
    -- remove all cards with "potion", "armor" and "sword" labels from deck
    -- foreach label, promprt which card to add

    local labels = {'sword', 'armor', 'potion'}

    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'At the start of the game, choose your gear. Select a POTION, ARMOR, and SWORD, and shuffle 2 copies of each into your deck.',
            function (args)
                -- TODO too low-level
                local cards = GetDeck(args.owner)
                local gear = {}
                for _, label in ipairs(labels) do
                    gear[label] = {}
                end
                -- {
                --     potion = {
                --         card1 = {
                --             mc1,
                --             mc2
                --         },
                --         card2 = {
                --             mc3,
                --             mc4
                --         }
                --     }
                -- }
                -- remove all gear from the deck
                for _, card in ipairs(cards) do
                    for _, label in ipairs(labels) do
                        if CardHasLabel(card, label) then
                            local gearTable = gear[label]
                            if gearTable[card.Template.Name] == nil then
                                gearTable[card.Template.Name] = {}
                            end
                            gearTable[card.Template.Name][#gearTable[card.Template.Name]+1] = card
                            RemoveCardFromDeck(args.owner, card)
                            break
                        end
                    end
                end
                DEBUGTABLE(gear)

                for _, gearType in ipairs(labels) do
                    local gearCards = gear[gearType]
                    local choiceMap = {}
                    local choices = {}
                    local logMsg = args.fighter.FormattedLogName..' chooses between '
                    for cardName, matchCards in pairs(gearCards) do
                        choiceMap[cardName] = matchCards
                        choices[#choices+1] = cardName
                        logMsg = logMsg..' '..matchCards[1].FormattedLogName -- TODO better log message
                    end
                    logMsg = logMsg..' for gear type '..gearType
                    LogPublic(logMsg)

                    local choice = ChooseString(args.owner, choices, 'Add which '..gearType..' to your deck?')
                    for _, card in ipairs(choiceMap[choice]) do
                        AddCardToDeck(args.owner, card)
                    end
                end
            end
        )
    :Build()
end