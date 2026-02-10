function _Create()
    return UM.Build:Card()
        :DuringCombat(
            'During combat: Your opponent discards 1 random card. Add its BOOST value to this card\'s value',
            UM.Effects:Discard(
                UM.Select:Players()
                    :YourOpponent()
                    :Build(),
                1, true, 'DISCARDED'
            ),
            function (args)
                local cards = args.ctx['DISCARDED']
                if #cards == 0 then
                    return
                end
                local boost = GetBoostValue(cards[1])
                if boost == nil then
                    return
                end

                AddToCardValueInCombat(args.owner, boost)
            end
        )
    :Build()
end