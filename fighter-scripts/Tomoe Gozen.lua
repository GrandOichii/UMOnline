
-- TODO too low-level

function _Create()
    return UM.Build:Fighter()
        :OnMove(
            'When an opposing hero leaves Tomoe Gozen\'s zone, deal 1 damage to that hero.',
            function (args, fighter, fromNode, toNode)
                if not UM.Select:Fighters()
                    :Opposing()
                    :BuildPredicate()(args, fighter)
                then
                    DEBUG('FIRST')

                    return
                end

                if not UM.Select:Nodes()
                    :InZoneOfFighter(
                        UM.Fighter:Named('Tomoe Gozen')
                    )
                    :BuildPredicate()(args, fromNode)
                then
                    DEBUG('SECOND')

                    return
                end

                if toNode ~= nil and UM.Select:Nodes()
                    :InZoneOfFighter(
                        UM.Fighter:Named('Tomoe Gozen')
                    )
                    :BuildPredicate()(args, toNode)
                then
                    DEBUG('THIRD')
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