function _Create()
    return UM.Build:Card()
        :Effect(
            'Choose any space in Merlin\'s zone. Deal 2 damage to each opposing fighter in that space and in one additional space. If at least one fighter is defeated this way, draw 1 card.',
            function (args)
                local firstSpace = UM.Select:Nodes()
                    :InZoneOfFighter(
                        UM.Fighter:Named('Merlin')
                    )
                    :Single()
                    :Build()(args, 'Choose any space in Merlin\'s zone')[1]
                local secondSpace = UM.Select:Nodes()
                    :AdjacentToRawNode(firstSpace)
                    :Single()
                    :Build()(args, 'Choose an adjacent space to the selected space')[1]
                
                local defeated = 0
                for _, space in ipairs({ firstSpace, secondSpace }) do
                    local target = UM.Select:Fighters()
                        :StandOnRawNode(space)
                        :Single()
                        :Build()(args, 'Choose a fighter to deal damage to')[1]
                    if target ~= nil then
                        DealDamage(target, 2)
                        if GetHealth(target) == 0 then
                            defeated = defeated + 1
                        end
                    end
                end

                if defeated == 0 then
                    return
                end

                UM.Effects:Draw(
                    UM.Select:Players():You():Build(),
                    UM.Number:Static(1)
                )(args)
            end
        )
    :Build()
end