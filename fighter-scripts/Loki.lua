-- If an opponent ever DISCARDS a trick card from their hand, you may choose to put the trick card into your hand or on top of your deck instead of your discard pile.

-- Loki add +1 to their move value for each trick card in all of your opponent's hands.

local function IsTrickCard(card)
    return CardHasLabel(card, 'trick')
end

function _Create()
    return UM.Build:Fighter()
        :RedirectCardZoneChange(
            'If an opponent discards a TRICK from their hand, return that card to your hand or the top of your deck.',
            function (args, zoneChange)
                local card = zoneChange.Card
                if not IsTrickCard(card) then
                    return false
                end

                if GetZoneChangeType(zoneChange) ~= UM.ZoneChange.Types.DISCARDED then
                    return false
                end

                local from = zoneChange.FromZone
                if from:GetOwner() == args.owner then
                    return false
                end

                local choice = ChooseString(args.owner, {'Hand', 'Top of deck'}, 'Where to place '..card.FormattedLogName..'?')

                local zone = GetZone(args.owner, 'HAND')
                if choice ~= 'Hand' then
                    zone = GetZone(args.owner, 'DECK')
                    SetZoneChangeLocation(zoneChange, 1)
                end

                ChangeTargetZone(zoneChange, zone)
                return true
            end
        )
        :RedirectCardZoneChange(
            'When your opponent plays one of your tricks, that card goes to your discard pile during the cleanup step.',
            function (args, zoneChange)
                local card = zoneChange.Card
                if not IsTrickCard(card) then
                    return false
                end

                if GetZoneChangeType(zoneChange) ~= UM.ZoneChange.Types.PLAYED then
                    return false
                end

                local from = zoneChange.FromZone
                if from:GetOwner() == args.owner then
                    return false
                end

                local zone = GetZone(args.owner, 'DISCARD')
                ChangeTargetZone(zoneChange, zone)

                return true
            end
        )
        :RedirectCardZoneChange(
            'After you play a TRICK, put that card into your opponent\'s hand instead of your discard pile.',
            function (args, zoneChange)
                local card = zoneChange.Card
                if not IsTrickCard(card) then
                    return false
                end

                if GetZoneChangeType(zoneChange) ~= UM.ZoneChange.Types.PLAYED then
                    return false
                end

                -- check if the card was played from a hand of the owner
                local from = zoneChange.FromZone
                if from:GetOwner() ~= args.owner then
                    return false
                end

                local opp = UM.Player:Opponent()(args, 'Choose who will receive '..card.FormattedLogName)
                local zone = GetZone(opp, 'HAND')
                ChangeTargetZone(zoneChange, zone)

                return true
            end
        )
        :ModManoeuvreValue(
            UM.Select:Fighters():AllYour():BuildPredicate(),
            function (args, subjects, original)
                return original + UM.Count.CharacterSpecific:TricksInOpponentsHands():Last(args)
            end
        )
    :Build()
end

function UM.Count.CharacterSpecific:TricksInOpponentsHands()
    return UM.Number:_(function (args)
        local opps = UM.Select:Players()
            :Opponents()
            :Build()(args)
        local result = 0

        for _, opp in ipairs(opps) do
            local cards = GetHand(opp)
            for _, card in ipairs(cards) do
                if IsTrickCard(card) then
                    result = result + 1
                end
            end
        end

        return {result}
    end)
end