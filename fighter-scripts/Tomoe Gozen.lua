
-- TODO too low-level

function _Create()
    return UM.Build:Fighter()
        :OnMove(
            'When an opposing hero leaves Tomoe Gozen\'s zone, deal 1 damage to that hero.',
            function (args, fighter, fromNode, toNode)
                if not UM.Select:Fighters()
                    :Opposing()
                    :BuildContains()(args, fighter)
                then
                    return
                end

                if not UM.Select:Nodes()
                    :InZoneOfFighter(
                        UM.Fighter:Named('Tomoe Gozen')
                    )
                    :BuildContains()(args, fromNode)
                then
                    return
                end

                if toNode ~= nil and UM.Select:Nodes()
                    :InZoneOfFighter(
                        UM.Fighter:Named('Tomoe Gozen')
                    )
                    :BuildContains()(args, toNode)
                then
                    return
                end

                UM.Effects:DealDamage(
                    UM.Select:Fighters()
                        :Only(function ()
                            return fighter
                        end)
                        :Build(),
                    UM.Number:Static(1)
                )(args)
            end
        )
    :Build()
end