
-- Resolve an effect on a card you play if the symbol next to the effect matches the item in your basket. At the start of the game, place LITTLE RED's BASKET in your discard pile.
-- Little Red's Basket: This starts in your discard pile. It does not count as a card.

function _Create()
    return UM.Build:Fighter()
    :Build()
end

-- TODO not tested
function UM.Conditions.CharacterSpecific:InBasket(symbol)
    return function (args)
        local cards = GetCardsInDiscardPile(args.owner)

        -- Little Red's Basket
        if #cards == 0 then
            return true
        end

        return CardHasLabel(cards[#cards], symbol)
    end
end