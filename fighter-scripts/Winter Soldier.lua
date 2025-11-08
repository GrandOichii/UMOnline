-- TODO too low level, isCardOfCharacter is duplicated from Sherlock Holmes

-- Effects on Winter Soldier's cards cannot be canceled.

function _Create()
    local isCardOfCharacter = function (name)
        return function (args, card)
            return IsCardOfCharacter(card, name)
        end
    end

    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'Start your turn with {RED ROOM} effects active',
            UM.Effects.CharacterSpecific:WSEnableRedRoom()
        )
        :ForbidCardCancelling(
            isCardOfCharacter('Winter Soldier'),
            UM.Select:Players():BuildPredicate()
        )
    :Build()
end

function UM.Conditions.CharacterSpecific:WSRedRoomActive()
    return function (args)
        return GetPlayerStringAttribute(args.owner, 'RED_ROOM_ACTIVE') == 'Y'
    end
end

function UM.Effects.CharacterSpecific:WSEnableRedRoom()
    return function (args)
        local prev = GetPlayerStringAttribute(args.owner, 'RED_ROOM_ACTIVE')
        if prev == 'Y' then
            return
        end

        SetPlayerStringAttribute(args.owner, 'RED_ROOM_ACTIVE', 'Y')

        LogPublic('Red room effects are now active for Winter Soldier') -- TODO format name
    end
end

function UM.Effects.CharacterSpecific:WSIngoreRedRoomForTheRestOfTheTurn()
    return function (args)
        SetPlayerStringAttribute(args.owner, 'RED_ROOM_ACTIVE', 'N')

        LogPublic('Red room effects are disabled for the rest of the turn')
    end
end