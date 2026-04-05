
function _Create()
    return UM.Build:Fighter()
        :ReplaceBoostedMovement(
            function (args, fighter)
                local houdini = UM.Fighter:Named('Houdini')
                if houdini(args) ~= fighter then
                    return false
                end

                local choice = ChooseString(args.owner, { 'Yes', 'No' }, 'Replace Houdini\'s movement?')
                if choice == 'No' then
                    return false
                end

                UM.Effects:PlaceFighter(
                    houdini,
                    UM.Select:Nodes()
                        :Empty()
                        :BuildOne()
                )(args)
                return true
            end
        )
    :Build()
end