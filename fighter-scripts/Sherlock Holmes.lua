-- TODO too low level

-- Effects on HOLMES and DR. WATSON cards cannot be cancelled by an opponent.

function _Create()
    return UM.Build:Fighter()
        :ForbidEffectCancelling(
            -- which cards cant be cancelled?
            function (args, card)
                return
                    card.Template:CanBePlayedBy('Sherlock Holmes') or
                    card.Template:CanBePlayedBy('Dr. Watson')
            end,
            UM.Select:Players():Opponents():BuildPredicate()
        )
    :Build()
end
