
function _Create()
    local jekyll = 'Dr. Jekyll'
    local hyde = 'Mr. Hyde'

    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Start the game as Dr. Jekyll.',
            UM.Effects.CharacterSpecific:ChangeToJHIdentity(jekyll)
        )
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may transform into Dr. Jekyll or Mr. Hyde.',
            function (args)
                local prev = GetPlayerStringAttribute(args.owner, 'IDENTITY')
                local newIdentity = jekyll
                if prev == newIdentity then
                    newIdentity = hyde
                end
                UM.Effects:Optional(
                    'Transform into '..newIdentity..'?',
                    UM.Effects.CharacterSpecific:ChangeToJHIdentity(newIdentity)
                )(args)
            end
        )
        :OnManoeuvre(
            'While Mr. Hyde: After you Maneuver, take 1 damage.',
            UM.Effects:If(
                UM.Conditions:PlayerAttributeEqualTo('IDENTITY', hyde),
                UM.Effects:DealDamage(
                    UM.Select:Fighters():Named('Mr. Hyde'):Build(),
                    UM.Number:Static(1)
                )
            )
        )
    :Build()
end

function UM.Effects.CharacterSpecific:ChangeToJHIdentity(identity)
    return function (args)
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