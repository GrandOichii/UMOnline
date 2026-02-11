function _Create()
    return UM.Build:Card()
        :DuringCombat(
            "During combat: This card's value is +1 for each other VOYAGE card in your discard pile.",
            UM.Effects:ModCombatCardValue(
                UM.Player:EffectOwner(),
                UM.Number:Count(
                    UM.Select:CardsInDiscardPile(UM.Player:EffectOwner())
                        :WithLabel("voyage")
                        :Build()
                ),
                1, true
            )
        )
        :AfterCombat(
            "After combat: Take all other VOYAGE cards from your discard pile and add them yo your hand.",
            function (args)
                local cards = UM.Select:CardsInDiscardPile(UM.Player:EffectOwner())
                    :WithLabel('voyage')
                    :Build()(args)

                for _, card in ipairs(cards) do
                    ReturnCardFromDiscardPile(args.owner, card.Id)
                end
            end
        )
        :Build()
end
