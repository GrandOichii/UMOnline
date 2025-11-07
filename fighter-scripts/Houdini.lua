
function _Create()
    return UM.Build:Fighter()
        :ReplaceBoostedMovement(
            function (args, fighter)
                -- TODO can Houdini stay in his place?
                local houdini = UM.Fighter:Named('Houdini')
                if houdini(args) ~= fighter then
                    return false
                end

                local choice = ChooseString(args.owner, {
                    [1] = 'Yes',
                    [2] = 'No'
                }, 'Replace Houdini\'s movement?')
                if choice == 'No' then
                    return false
                end

                UM.Effects:PlaceFighter(
                    houdini,
                    UM.Select:Nodes()
                        :Empty()
                        :Build()
                )(args)
                return true
            end
        )
    :Build()
end