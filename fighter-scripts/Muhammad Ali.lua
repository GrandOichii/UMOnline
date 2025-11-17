-- .  (Float Like A Butterfly: you can attack from 2 spaces away; Sting Like A Bee: add +2 to your attacks)

local floatLikeAButterfly = 'Float Like A Butterfly'
local stingLikeABee = 'Sting Like A Bee'

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Begin the game with your stance on Float Like a Butterfly.',
            UM.Effects.CharacterSpecific:SetStance('blaster')
        )
        :AfterAttack(
            'After you attack, if you won the combat, change stances.',
            UM.Select:Fighters():Named('Muhammad Ali'):BuildPredicate(),
            UM.Effects:If(
                UM.Conditions:CombatWonBy(UM.Player:EffectOwner()),
                UM.Effects.CharacterSpecific:ChangeStance()
            )
        )
    :Build()
end

function UM.Effects.CharacterSpecific:SetStance(stance)
    return function (args)
        local prev = GetPlayerStringAttribute(args.owner, 'STANCE')
        SetPlayerStringAttribute(args.owner, 'STANCE', stance)
        local fighter = UM.Fighter:Named('Muhammad Ali')(args)
        if prev == nil then
            LogPublic(fighter.FormattedLogName..' sets his stance to '..stance)
            return
        end
        if prev ~= stance then
            LogPublic(fighter.FormattedLogName..' changes his stance to '..stance)
            return
        end
        -- prev and stance are equal
    end
end

function UM.Effects.CharacterSpecific:ChangeStance()
    return function (args)
        local prev = GetPlayerStringAttribute(args.owner, 'STANCE')
        local newStance = floatLikeAButterfly
        if prev == newStance then
            newStance = stingLikeABee
        end

        UM.Effects.CharacterSpecific:SetStance(newStance)(args)
    end
end