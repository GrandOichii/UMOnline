
local floatLikeAButterfly = 'Float Like A Butterfly'
local stingLikeABee = 'Sting Like A Bee'

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Begin the game with your stance on Float Like a Butterfly.',
            UM.Effects.CharacterSpecific:SetStance(floatLikeAButterfly)
        )
        :AfterAttack(
            'After you attack, if you won the combat, change stances.',
            UM.Select:Fighters():Named('Muhammad Ali'):BuildPredicate(),
            UM.Effects:If(
                UM.Conditions:CombatWonBy(UM.Player:EffectOwner()),
                UM.Effects.CharacterSpecific:ChangeStance()
            )
        )
        :ModCardValue(
            'Sting Like A Bee: add +2 to your attacks.',
            UM.Select:Fighters():Named('Muhammad Ali'):BuildPredicate(),
            UM.Mod.Cards:AttackCards(UM.Number:Static(2)),
            UM.Conditions.CharacterSpecific:StanceIsActive(stingLikeABee)
        )
        :ModMeleeRange(
            'Float Like A Butterfly: you can attack from 2 spaces away',
            {
                UM.Select:Fighters():Named('Muhammad Ali'):BuildPredicate(),
                UM.Conditions.CharacterSpecific:StanceIsActive(floatLikeAButterfly)
            },
            function (args, subjects, original)
                return 2
            end
        )
    :Build()
end

function UM.Conditions.CharacterSpecific:StanceIsActive(stance)
    return function (args)
        return GetPlayerStringAttribute(args.owner, 'STANCE') == stance
    end
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