
function _Create()
    return UM.Build:Fighter()
        :OnLostCombat(
            'On each of your turns, the first time you lose combat, gain 1 action.',
            function (args, subjects)
                local player = subjects.player
                if player ~= args.owner then
                    DEBUG('NOT OWNER')
                    return
                end

                if UM.Player:Current()(args) ~= player then
                    DEBUG('NOT CURRENT')

                    return
                end

                local lostCounter = GetLostCounter(args.owner)
                if lostCounter > 1 then
                    DEBUG('BAD LOST COUNTER '..tostring(lostCounter))

                    return
                end

                UM.Effects:GainActions(1)(args)
            end
        )
    :Build()
end