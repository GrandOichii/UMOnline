function _Create()
    return UM.Build:Card()
        :Effect(
            'Search your deck and discard pile for the EXCALIBUR card. Add it to your hand. If you searched your deck, shuffle it.',
            function(args)
                local card = UM.Select:CardsInDiscardPile(UM.Player:EffectOwner())
                    :Named('Excalibur')
                    :Build()(args)[1]

                if card ~= nil then
                    ReturnCardFromDiscardPile(args.owner, card.Id)
                    return
                end
                card = UM.Select:CardsInDeck(UM.Player:EffectOwner())
                    :Named('Excalibur')
                    :Build()(args)[1]

                if card == nil then
                    -- could be in hand OR in Black Panther's vibranium suit
                    return
                end

                MoveCard(
                    card,
                    args.owner.Deck,
                    args.owner.Hand
                )
            end
        )
        :Build()
end
