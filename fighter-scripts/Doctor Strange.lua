
function _Create()
    return UM.Build:Fighter()
        :AfterCombat(
            'After each combat, if Doctor Strange played a card, you may deal 1 damage to him. If you do, put that card on the bottom of your deck and draw 1 card.',
            UM.Effects:If(
                UM.Conditions:PlayedCombatCard(
                    UM.Player:EffectOwner()
                ),
                UM.Effects:Optional(
                    'Deal 1 damage to Doctor Strange?',
                    UM.Effects:DealDamage(
                        UM.Select:Fighters():Named('Doctor Strange'):Build(),
                        UM.Number:Static(1)
                    ),
                    -- TODO too low-level
                    function (args)
                        local player = args.owner
                        local part = RemoveCombatPart(player)
                        PutCardOnTheBottomOfDeck(player, part[1], part[1].Card)
                    end
                )
            )
        )
    :Build()
end