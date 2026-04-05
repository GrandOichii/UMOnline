-- TODO finish
-- If you do not have a face-up glamour at the start of your turn, flip the top card of your glamour deck face-up.
-- Its effect is ongoing while it remains face-up.

-- Glamour:


-- 

function _Create()
    return UM.Build:Fighter()
        -- Glamour of Love – 
        :BeforeManoeuvre(
            'When you maneuver, you may move an opposing fighter 1 space before moving your fighters.',
            {
                UM.Select:Players():You():BuildPredicate(),
                UM.Conditions.CharacterSpecific:GlamourRevealed('Glamour of Love')
            },
            UM.Effects:MoveFighters(
                UM.Select:Fighters():Opposing():Single():Build(),
                UM.Number:Static(1), false
            )
        )
        -- Glamour of Greed
        :AtTheEndOfYourTurn(
            'At the end of your turn, you may draw 1 card.',
            {
                UM.Conditions.CharacterSpecific:GlamourRevealed('Glamour of Greed')
            },
            UM.Effects:Optional(
                'Draw 1 card?',
                UM.Effects:Draw(
                    UM.Select:Players():You():Build(),
                    UM.Number:Static(1)
                )
            )
        )
        -- Glamour of Sleep
        :ModifyDamage(
            -- Your fighters do not take damage other than combat damage.
            function (args, fighter, isCombatDamage, damage)
                if not UM.Conditions.CharacterSpecific:GlamourRevealed('Glamour of Sleep')(args) then
                    return damage
                end
                if isCombatDamage then
                    return damage
                end
                if not UM.Select:Fighters():AllYour():BuildContains()(args, fighter) then
                    return damage
                end
                return 0
            end
        )
        -- Glamour of Jealousy
        :OnCombatCardChoice(
            'When Titania or Oberon are attacked, before playing a card, they may swap spaces. If they do, your other fighter is now the defender.',
            function (args, subjects)
                if not UM.Conditions.CharacterSpecific:GlamourRevealed('Glamour of Jealousy')(args) then
                    return
                end
                -- TODO check that Titiania or Oberon are attacked
                -- TODO prompt to swap places
            end
        )
        -- Glamour of Invisibility – Your fighters may move through opposing fighters.
        -- TODO
        -- Glamour of Rhyme
        :AfterScheme(
            'After you play a scheme, gain 1 action.',
            {
                UM.Select:Fighters():AllYour():BuildPredicate(),
                UM.Conditions.CharacterSpecific:GlamourRevealed('Glamour of Rhyme')
            },
            UM.Effects:GainActions(1)
        )
        :Build()
end

function UM.Conditions.CharacterSpecific:GlamourRevealed(name)
    return function (args)
        -- TODO
    end
end