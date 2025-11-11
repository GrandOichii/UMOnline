
function _Create()
    return UM.Build:Fighter()
        :OnLostCombat(
            'On each of your turns, the first time you lose combat, gain 1 action.',
            function (args, subjects)
                local player = subjects.player
                if player ~= args.owner then
                    return
                end

                if UM.Player:Current()(args) ~= player then
                    return
                end

                local lostCounter = GetLostCounter(args.owner)
                if lostCounter > 1 then
                    return
                end

                UM.Effects:GainActions(1)(args)
            end
        )
    :Build()
end