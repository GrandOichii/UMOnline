
-- 
-- TO BE: When you maneuver, draw 1 additional card.
-- NOT TO BE: Add +2 to the value of Hamlet's attacks.

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, choose TO BE or NOT TO BE. If you choose NOT TO BE, deal 2 damage to one of your fighters.',
            function (args)
                -- TODO too low-level
                local choice = ChooseString(args.owner, {
                    [1] = 'TO BE',
                    [2] = 'NOT TO BE',
                }, 'TO BE or NOT TO BE?')

                UM.Effects.CharacterSpecific:SetHamletAttr(choice)(args)

                if choice == 'TO BE' then
                    return
                end

                UM.Effects:DealDamage(
                    UM.Select:Fighters():AllYour():Single():Build(),
                    UM.Number:Static(2)
                )(args)
            end
        )
        :ModManoeuvreCardDraw(
            -- TODO too low-level
            function (args, player, resultValue)
                if player ~= args.owner then
                    return resultValue
                end
                if not UM.Conditions.CharacterSpecific:ToBe()(args) then
                    return resultValue
                end

                return resultValue + 1
            end
        )
        :ModCardValue(
            UM.Select:Fighters():Named('Hamlet'):BuildPredicate(),
            UM.Mod.Cards:AllCards(UM.Number:Static(2)),
            UM.Conditions.CharacterSpecific:NotToBe()
        )
        :Build()
end

function UM.Effects.CharacterSpecific:SetHamletAttr(value)
    return function (args)
        SetPlayerStringAttribute(args.owner, 'TOBE', value)
        LogPublic(args.owner.FormattedLogName..' chose '..value)
    end
end

function UM.Conditions.CharacterSpecific:ToBe()
    return UM.Conditions.CharacterSpecific:HamletAttrEq('TO BE')
end

function UM.Conditions.CharacterSpecific:NotToBe()
    return UM.Conditions.CharacterSpecific:HamletAttrEq('NOT TO BE')
end

function UM.Conditions.CharacterSpecific:HamletAttrEq(value)
    return function (args)
        local v = GetPlayerStringAttribute(args.owner, 'TOBE')
        return v == value
    end
end