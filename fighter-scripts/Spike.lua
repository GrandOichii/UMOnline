function _Create()
    local nodeSelector = function ()
        return UM.Select:Nodes()
            :AdjacentToFighters(
                UM.Select:Fighters()
                    :Named('Spike', 'Drusilla')
                    :Build()
            )
    end

    return UM.Build:Fighter()
        :DeclareToken(
            'Shadow',
            UM.Build:Token()
                :Amount(3)
                :Build()
        )
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may place a Shadow token in any space adjacent to Spike or Drusilla.',
            UM.Effects:If(
                UM.Conditions:CountGte(nodeSelector():Build(), 1),
                UM.Effects:IfInstead(
                    UM.Conditions:TokensLeft('Shadow'),
                    UM.Effects:Optional(
                        'Place a Shadow token in a space adjacent to Spike or Drusilla?',
                        UM.Effects:PlaceTokens(
                            'Shadow',
                            nodeSelector():Single():Build()
                        )
                    ),
                    UM.Effects:Optional(
                        'Move a Shadow token to a space adjacent to Spike or Drusilla?',
                        UM.Effects:MoveToken(
                            UM.Select:Tokens():Named('Shadow'):BuildOne(),
                            nodeSelector():Single():BuildOne()
                        )
                    )
                )
            )
        )
    :Build()
end
