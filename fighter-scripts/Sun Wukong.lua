
function _Create()
    local nodeSelector = function ()
        return UM.Select:Nodes()
            :Empty()
            :AdjacentToFighters(
                UM.Select:Fighters()
                    :Named('Sun Wukong')
                    :Build()
            )
    end

    local defeatedFighterSelector = function ()
        return UM.Select:Fighters():Named('Clone'):Defeated()
    end

    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may take 1 damage to summon a Clone in an empty space adjacent to Sun Wukong.',
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions:CountGte(defeatedFighterSelector():Build(), 1),
                    UM.Conditions:CountGte(nodeSelector():Build(), 1)
                ),
                UM.Effects:Optional(
                    'Take 1 damage and summon a Clone?',
                    UM.Effects:DealDamage(
                        UM.Select:Fighters():Named('Sun Wukong'):Build(),
                        UM.Number:Static(1)
                    ),
                    UM.Effects:ReviveAndSummon(
                        defeatedFighterSelector():BuildFirst(),
                        nodeSelector():BuildOne()
                    )
                )
            )
        )
    :Build()
end