
function _Create()
    return UM.Build:Fighter()
        :WhenPlaced(
            'When you place Alice, choose whether she starts the game BIG or SMALL',
            function (args)
                local choice = ChooseString(args.owner, {
                    [1] = 'BIG',
                    [2] = 'SMALL',
                }, 'Start the game BIG or SMALL?')
                UM.Effects.CharacterSpecific._SetSize(args.owner, choice)
            end
        )
        :ModCardValue(
            UM.Mod.Cards:AttackCards(UM.Number:Static(2)),
            UM.Conditions:PlayerAttributeEqualTo('ALICE_SIZE', 'BIG')
        )
        :ModCardValue(
            UM.Mod.Cards:DefenseCards(UM.Number:Static(1)),
            UM.Conditions:PlayerAttributeEqualTo('ALICE_SIZE', 'SMALL')
        )
    :Build()
end

function UM.Effects.CharacterSpecific._SetSize(owner, size)
    SetPlayerStringAttribute(owner, 'ALICE_SIZE', size)

    LogPublic('Alice changes size! Alice is now '..size)
end

function UM.Effects.CharacterSpecific:ChangeSize()
    return function (args)
        local attr = GetPlayerStringAttribute(args.owner, 'ALICE_SIZE')
        local newSize = 'BIG'
        if attr == 'BIG' then
            newSize = 'SMALL'
        end
        UM.Effects.CharacterSpecific._SetSize(args.owner, newSize)
    end
end