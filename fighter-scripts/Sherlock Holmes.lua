-- TODO too low level

-- Effects on HOLMES and DR. WATSON cards cannot be cancelled by an opponent.

function _Create()
    local isCardOfCharacter = function (name)
        return function (args, card)
            return IsCardOfCharacter(card, name)
        end
    end

    local orCond = function (cond1, cond2)
        return function (args, card)
            return cond1(args, card) or cond2(args, card)
        end
    end

    return UM.Build:Fighter()
        :ForbidCardCancelling(
            -- orCond(
            --     isCardOfCharacter('Sherlock Holmes'),
            --     isCardOfCharacter('Dr. Watson')
            -- ),
            function (args, card)
                if IsCardOfCharacter(card, 'Dr. Watson') then
                    return true
                end
                if IsCardOfCharacter(card, 'Sherlock Holmes') then
                    return true
                end
                DEBUG('FAILED FIGHTER COND')
                return false
            end,
            function (args, player)
                return args.owner ~= player
            end
            -- UM.Select:Players():Opponents():BuildPredicate()
        )
    :Build()
end
