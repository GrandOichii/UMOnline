function _Create()
    return UM.Build:Card()
        :Effect(
            'Look at the top 4 cards of your deck. Add 2 of them to your hand and put the other 2 back on top of your deck, in any order.',
            function (args)
                local cards = GetTopCardsOfDeck(args.owner, 4)
                assert(#cards <= 4, 'GetTopCardsOfDeck returned more cards than requested')
                local count = #cards

                local toHandCount = 2
                if toHandCount > count then
                    toHandCount = count
                end

                count = count - toHandCount
                if toHandCount == 0 then
                    return
                end
                local toTopCount = 2
                if toTopCount > count then
                    toTopCount = count
                end

                assert(toHandCount <= 2)
                assert(toTopCount <= 2)
                assert(toHandCount + toTopCount <= 4)

                while toHandCount > 0 do
                    local names = {}
                    local nameMap = {}
                    for _, card in ipairs(cards) do
                        names[#names+1] = card.FormattedLogName
                        nameMap[card.FormattedLogName] = card
                    end
                    local choice = ChooseString(args.owner, names, 'Choose a card to put to your hand')
                    local card = nameMap[choice]
                    MoveCard(card, args.owner.Deck, args.owner.Deck)
                    
                    toHandCount = toHandCount - 1
                    cards = GetTopCardsOfDeck(args.owner, toHandCount + toTopCount)
                end

                if toTopCount == 0 or toTopCount == 1 then
                    return
                end

                local names = {}
                local nameMap = {}
                for _, card in ipairs(cards) do
                    names[#names+1] = card.FormattedLogName
                    nameMap[card.FormattedLogName] = card
                end
                local choice = ChooseString(args.owner, names, 'Choose a card to put on top')
                local card = nameMap[choice]
                MoveCardToTopOfDeck(card)
            end
        )
    :Build()
end