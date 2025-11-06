
function _Create()

    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Start the game as Willow.',
            UM.Effects.CharacterSpecific:ToggleDarkWillow(false)
        )
        :AtTheEndOfYourTurn(
            'At the end of your turn, if Dark Willow is adjacent to Tara, she becomes Willow.',
            UM.Effects:If(
                UM.Conditions:FightersAreAdjacent(
                    UM.Select:Fighters():Named('Dark Willow'):BuildOne(),
                    UM.Select:Fighters():Named('Tara'):BuildOne()
                ),
                UM.Effects.CharacterSpecific:ToggleDarkWillow(false)
            )
        )
        :OnDamage(
            'When Willow is dealt damage, Willow becomes Dark Willow',
            UM.Effects.CharacterSpecific:ToggleDarkWillow(true)
        )

    :Build()
end

function UM.Effects.CharacterSpecific:ToggleDarkWillow(isDarkWillow)
    local identityMap = {
        [false] = 'Willow',
        [true] = 'Dark Willow',
    }

    return function (args)
        local identity = identityMap[isDarkWillow]
        local prev = GetPlayerStringAttribute(args.owner, 'IDENTITY')
        if prev == identity then
            return
        end

        SetPlayerStringAttribute(args.owner, 'IDENTITY', identity)
        SetFighterName(args.fighter, identity)

        if prev == nil then
            return
        end
        LogPublic(prev..' transforms into '..identity..'!')
    end
end