--  
-- 
function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Ghost Rider starts the game with 5 Hellfire.',
            UM.Effects.CharacterSpecific:SetHellfire(5)
        )
        :WhenManoeuvre(
            'When you maneuver you may spend 1 Hellfire. If you do, increase Ghost Rider\'s move value to 4, and he mave move through opposing fighters. Then deal 1 damage to each opposing fighter he moved through.',
            UM.Select:Fighters():Named('Ghost Rider'):BuildPredicate(),
            UM.Effects:If(
                UM.Conditions.CharacterSpecific:HasAnyHellfire(),
                UM.Effects:Optional(
                    'Spend 1 Hellfire?',
                    UM.Conditions.CharacterSpecific:SpendHellfire(1),
                    function (args)
                        -- TODO too low-level
                        local movement = GetcurrentMovement()
                        movement.CanMoveOverOpposing = true
                        movement.Value = 4

                        AddAtTheEndOfMovementEffect(args.fighter, UM.Build:EffectCollection()
                            :SourceIsAlive()
                            :Text('Deal 1 damage to each opposing fighter he moved through.')
                            :Effects({
                                [1] = UM.Effects:DealDamage(
                                    UM.Select:Fighters():MovedThrough():Build(),
                                    UM.Number:Static(1)
                                )
                            })
                            :Build()
                        )
                    end
                )
            )
        )
    :Build()
end

function UM.Conditions.CharacterSpecific:SpendHellfire(amount)
    return function (args)
        local prev = GetPlayerIntAttribute(args.owner, 'HELLFIRE')
        if prev - amount < 0 then
            error('Tried to spend '..tostring(amount)..' Hellfire while having only '..tostring(prev)..' Hellfire')
        end
        UM.Effects.CharacterSpecific:SetHellfire(prev - amount)(args)
    end
end

function UM.Conditions.CharacterSpecific:HasAnyHellfire()
    return function (args)
        return GetPlayerIntAttribute(args.owner, 'HELLFIRE') > 0
    end
end

function UM.Effects.CharacterSpecific:SetHellfire(value)
    return function (args)
        local prev = GetPlayerIntAttribute(args.owner, 'HELLFIRE')
        if value > 5 then
            value = 5
        end
        if value < 0 then
            value = 0
        end
        if prev == value then
            return
        end
        SetPlayerIntAttribute(args.owner, 'HELLFIRE', value)
        LogPublic(args.owner.FormattedLogName..' Hellfire is set to '..tostring(value))
    end
end

