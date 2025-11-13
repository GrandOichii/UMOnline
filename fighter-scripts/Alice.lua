
function _Create()
    return UM.Build:Fighter()
        :WhenPlaced(
            'When you place Alice, choose whether she starts the game BIG or SMALL',
            function (args)
                local choice = ChooseString(args.owner, {
                    [1] = 'BIG',
                    [2] = 'SMALL',
                }, 'Start the game BIG or SMALL?')
                UM.Effects.CharacterSpecific.SetAliceSize(args, choice)
            end
        )
        :ModCardValue(
            'When Alice is BIG, add 2 to the value of her attack cards.',
            UM.Select:Fighters():Named('Alice'):BuildPredicate(),
            UM.Mod.Cards:AttackCards(UM.Number:Static(2)),
            UM.Conditions:PlayerAttributeEqualTo('ALICE_SIZE', 'BIG')
        )
        :ModCardValue(
            'When Alice is SMALL, add 1 to the value of her defense cards.',
            UM.Select:Fighters():Named('Alice'):BuildPredicate(),
            UM.Mod.Cards:DefenseCards(UM.Number:Static(1)),
            UM.Conditions:PlayerAttributeEqualTo('ALICE_SIZE', 'SMALL')
        )
    :Build()
end

function UM.Effects.CharacterSpecific.SetAliceSize(args, size)
    SetPlayerStringAttribute(args.owner, 'ALICE_SIZE', size)
    local fighter = UM.Fighter:Named('Alice')(args)
    LogPublic(fighter.FormattedLogName..' changes size!'..fighter.FormattedLogName..' is now '..size)
end

function UM.Effects.CharacterSpecific:ChangeSize()
    return function (args)
        local attr = GetPlayerStringAttribute(args.owner, 'ALICE_SIZE')
        local newSize = 'BIG'
        if attr == 'BIG' then
            newSize = 'SMALL'
        end
        UM.Effects.CharacterSpecific.SetAliceSize(args, newSize)
    end
end