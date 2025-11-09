

function _Create()
    local fighterSelect = function ()
        return UM.Select:Fighters()
            :Opposing()
            :AdjacentTo(UM.Fighter:Named('Nikola Tesla'))
            :Build()
    end

    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Start the game with 1 coil charged.',
            UM.Effects.CharacterSpecific:ChargeCoil(1)
        )
        :AtTheEndOfYourTurn(
            'At the end of your turn, charge 1 coil.',
            UM.Effects.CharacterSpecific:ChargeCoil(1)
        )
        :AtTheStartOfYourTurn(
            'At the start of your turn, if both coils are charged, deal 1 damage to each opposing fighter adjacent to Tesla and move them up to 1 space.',
            UM.Effects:If(
                UM.Conditions.CharacterSpecific:CoilsCharged(2),
                UM.Effects:DealDamage(fighterSelect(), UM.Number:Static(1)),
                UM.Effects:MoveFighters(fighterSelect(), UM.Number:UpTo(1), false)
            )
        )
    :Build()
end

function UM.Conditions.CharacterSpecific:CoilsCharged(amount)
    return function (args)
        local charged = GetPlayerIntAttribute(args.owner, 'COILS')
        return charged >= amount
    end
end

function UM.Effects.CharacterSpecific:ChargeCoil(amount)
    return function (args)
        local prev = GetPlayerIntAttribute(args.owner, 'COILS')
        if prev == nil then
            prev = 0
        end
        local target = prev
        target = target + amount
        if target > 2 then
            target = 2
        end
        if target < 0 then
            target = 0
        end
        local charged = target - prev
        SetPlayerIntAttribute(args.owner, 'COILS', target)

        LogPublic('Nikola Tesla charges '..tostring(charged)..' coils, charge is at '..tostring(target)) -- TODO format name
    end
end