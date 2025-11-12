-- TODO cant place a fog token under Invisible Man

function _Create()
    local placeTokenEffect = UM.Effects:PlaceTokens(
        'Fog',
        UM.Select:Nodes()
            :InZoneOfFighter(UM.Fighter:Named('Invisible Man'))
            :WithNoToken('Fog')
            :Single()
            :Build()
    )
    return UM.Build:Fighter()
        :DeclareToken(
            'Fog',
            UM.Build:Token()
                :Amount(3)
                :Build()
        )
        :AtTheStartOfTheGame(
            'At the start of the game, after you place Invisible Man, place 3 fog tokens in separate spaces in his zone.',
            placeTokenEffect,
            placeTokenEffect,
            placeTokenEffect
        )
        :ModCardValue(
            'When Invisible Man is on a space with a fog token, add 1 to the value of his defense cards.',
            UM.Select:Fighters():Named('Invisible Man'):BuildPredicate(),
            UM.Mod.Cards:DefenseCards(UM.Number:Static(1)),
            UM.Conditions:FighterStandsOnNode(
                UM.Fighter:Source(),
                UM.Select:Nodes():WithToken('Fog'):Build()
            )
        )
        :ConnectNodesForMovement(
            UM.Select:Fighters():Named('Invisible Man'):BuildContains(),
            UM.Select:Nodes():WithToken('Fog'):BuildContains(),
            UM.Select:Nodes():WithToken('Fog'):Build()
        )
    :Build()
end
