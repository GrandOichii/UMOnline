
function _Create()
    local fighterSelect = function ()
        return UM.Select:Fighters()
            :InSameZoneAs(
                UM.Fighter:Named('She-Hulk')
            )
            :Except(UM.Fighter:Named('She-Hulk'))
    end

    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may discard a card to deal damage equal to its BOOST value to a fighter in your zone.',
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions:CountGte(fighterSelect():Build(), 1),
                    UM.Conditions:Gte(
                        UM.Count:CardsInHand(
                            UM.Player:EffectOwner()
                        ),
                        UM.Number:Static(1)
                    )
                ),
                UM.Effects:Optional(
                    'Discard a card to deal BOOST damage to a fighter in your zone?',
                    UM.Effects:Discard(
                        UM.Select:Players():You():Build(), 1, false, 'DISCARDED'
                    ),
                    UM.Effects:DealDamage(
                        fighterSelect():Single():Build(),
                        -- TODO too low-level
                        UM.Number:_(
                            function (args)
                                local discarded = args.ctx['DISCARDED']
                                DEBUG(tostring(#discarded))
                                if #discarded == 0 then
                                    return {
                                        [1] = 0
                                    }
                                end

                                return {
                                    [1] = GetBoostValue(discarded[1])
                                }
                            end
                        )
                    )
                )
            )
        )
    :Build()
end