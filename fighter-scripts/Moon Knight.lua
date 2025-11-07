
-- Moon Knight
-- At the start of your turn, move up to 2 spaces.

-- Khonshu
-- Khonshu adds +2 to the value of his attack cards. He does not take damage from effects other than combat damage.

-- Mr. Knight
-- Mr. Knight adds +1 to all his defense values.

function _Create()

    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Start the game as Moon Knight.',
            UM.Effects.CharacterSpecific:ChangeToMNIdentity('Moon Knight')
        )
        :AtTheEndOfYourTurn(
            'At the end of your turn, change to your next identity (In order, Moon Knight -> Khonshu -> Mr. Knight, repeating)',
            UM.Effects.CharacterSpecific:CycleMNIdentity()
        )
        -- Moon Knight
        :AtTheStartOfYourTurn(
            'At the start of your turn, move up to 2 spaces.',
            UM.Effects:If(
                UM.Conditions.CharacterSpecific:MNIdentityEq('Moon Knight'),
                UM.Effects:MoveFighters(
                    UM.Select:Fighters():Named('Moon Knight'):Build(),
                    UM.Number:UpTo(2),
                    false
                )
            )
        )
        -- Khonshu
        :ModCardValue(
            UM.Mod.Cards:AttackCards(UM.Number:Static(2)),
            UM.Conditions.CharacterSpecific:MNIdentityEq('Khonshu')
        )
        :ModifyDamage(
            -- TODO too low-level
            function (args, fighter, isCombatDamage, damage)
                if isCombatDamage then
                    return damage
                end
                if not UM.Select:Fighters():Named('Khonshu'):BuildPredicate()(args, fighter) then
                    return damage
                end
                return 0
            end
        )
        -- Mr. Knight
        :ModCardValue(
            UM.Mod.Cards:DefenseCards(UM.Number:Static(1)),
            UM.Conditions.CharacterSpecific:MNIdentityEq('Mr. Knight')
        )
    :Build()
end

function UM.Conditions.CharacterSpecific:MNIdentityEq(identity)
    return function (args)
        local cur = GetPlayerStringAttribute(args.owner, 'IDENTITY')
        return cur == identity
    end
end

function UM.Effects.CharacterSpecific:ChangeToMNIdentity(identity)
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
        LogPublic(prev..' changes to '..identity..'!')
    end
end

function UM.Effects.CharacterSpecific:CycleMNIdentity()
    local identities = {
        [1] = 'Moon Knight',
        [2] = 'Khonshu',
        [3] = 'Mr. Knight',
    }

    local getIdentityIdx = function (id)
        for i, iden in ipairs(identities) do
            if iden == id then
                return i
            end
        end
        error('Unrecognized Moon Knight identity: '..id)
    end

    return function (args)
        local prev = GetPlayerStringAttribute(args.owner, 'IDENTITY')
        local i = math.fmod(getIdentityIdx(prev), #identities) + 1
        local identity = identities[i]
        UM.Effects.CharacterSpecific:ChangeToMNIdentity(identity)(args)
    end
end