

-- When you place Alice, choose whether she starts the game BIG or SMALL.
-- When Alice is BIG, add 2 to the value of her attack cards.
-- When Alice is SMALL, add 1 to the value of her defense cards.

function _Create()
    return UM.Build:Fighter()
        -- :DefinePlayerAttribute(
        --     'ALICE_SIZE'
        -- )
        -- :WhenPlaced(
        --     'When you place Alice, choose whether she starts the game BIG or SMALL',
        --     function (args)
        --         local choice = ChooseString(args.owner, 'Start the game BIG or SMALL?', {
        --             [1] = 'BIG',
        --             [2] = 'SMALL',
        --         })
        --         SetPlayerAttribute(args.owner, 'ALICE_SIZE', choice)
        --     end
        -- )
        :ModCardValue(
            UM.Mod.Cards:AttackCards(2),
            UM.Conditions:PlayerAttributeEqualTo('ALICE_SIZE', 'BIG')
        )
        :ModCardValue(
            UM.Mod.Cards:DefenseCards(1),
            UM.Conditions:PlayerAttributeEqualTo('ALICE_SIZE', 'SMALL')
        )
    :Build()
end

function UM.Effects.CharacterSpecific:ChangeSize()
    return function (args)
        local attr = GetPlayerAttribute(args.owner, 'ALICE_SIZE')
        local newSize = 'BIG'
        if attr == 'BIG' then
            newSize = 'SMALL'
        end
        SetPlayerAttribute(args.owner, 'ALICE_SIZE', newSize)

        LogPublic('Alice changes size! Alice is now '..newSize)
    end
end