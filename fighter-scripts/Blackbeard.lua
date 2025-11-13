-- - 

-- BLACKBEARD'S DOUBLOONS
-- Doubloons that Blackbeard doesn't have are kept in the Treasury.

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Start the game with 1 doubloon in the treasury, you have the other 2.',
            UM.Effects.CharacterSpecific:_SetDoubloons(2)
        )
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may pay 1 doubloon to gain 1 action.',
            UM.Effects:If(
                UM.Conditions.CharacterSpecific:HasDoubloons(),
                UM.Effects:Optional(
                    'Pay 1 doubloon to gain 1 action?',
                    UM.Effects.CharacterSpecific:LoseDoubloon(),
                    UM.Effects:GainActions(1)
                )
            )
        )
        :OnDamage(
            'When Blackbeard takes combat damage, pay 1 doubloon.',
            UM.Select:Fighters():Named('Blackbeard'):Build(),
            UM.Effects:If(
                UM.Conditions.CharacterSpecific:HasDoubloons(),
                UM.Effects.CharacterSpecific:LoseDoubloon()
            )
        )
    :Build()
end

function UM.Effects.CharacterSpecific:_SetDoubloons(value)
    return function (args)
        local prev = GetPlayerIntAttribute(args.owner, 'DOUBLOONS')
        if value > 3 then
            value = 3
        end
        if value < 0 then
            value = 0
        end
        if prev == value then
            return
        end
        SetPlayerIntAttribute(args.owner, 'DOUBLOONS', value)
    end
end

function UM.Conditions.CharacterSpecific:HasDoubloons()
    return function (args)
        return GetPlayerIntAttribute(args.owner, 'DOUBLOONS') > 0
    end
end

function UM.Effects.CharacterSpecific:LoseDoubloon()
    return  function (args)
        -- TODO
        local prev = GetPlayerIntAttribute(args.owner, 'DOUBLOONS')
        if prev == 0 then
            error('Tried to lose doubloon when player has none')
        end
        LogPublic(args.owner.FormattedLogName..' pays 1 doubloon to the treasury.')
        UM.Effects.CharacterSpecific:_SetDoubloons(prev - 1)(args)
    end
end