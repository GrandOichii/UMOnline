-- TODO finish
-- PANDORA'S BOX
-- Do not start with any Kakodaemon on the board.
-- 

-- Pandora's Box is a deck of seven cards called MISERIES.
-- When you open Pandora's Box, reveal the top card and resolve its effect if any). You may keep revealing and resolving additional cards, one at a time, until you choose to stop.
-- If there are three or more total feathers on revealed cards, you must stop revealing, then Pandora takes 1 damage for each revealed MISERY.
-- At the end of your turn, shuffle all revealed MISERIES back into
-- Pandora's Box.


local function _defineMisery(name, feathers, text, ...)
    return {
        feathers = feathers,
        name = name,
        key = 'Pandora_'..name,
        effects = UM.Build:EffectCollection()
            :Effects({...})
            :Text(text)
            :Build()
    }
end


-- MISERIES

function _Create()
    local nodeSelector = function ()
        return UM.Select:Nodes()
            :InZoneOfFighter(UM.Fighter:Named('Pandora'))
            :Empty()
    end

    local defeatedFighterSelector = function ()
        return UM.Select:Fighters():Named('Clone'):Defeated()
    end

    local miseries = {
        _defineMisery(
            'Pain', 1,
            'Deal 1 damage to each opposing fighter adjacent to Pandora.',
            UM.Effects:DealDamage(
                UM.Select:Fighters()
                    :AdjacentTo(UM.Fighter:Named('Pandora'))
                    :Build(),
                UM.Number:Static(1)
            )
        ),
        _defineMisery(
            'Toil', 1,
            'Add +1 to the value of Pandora\'s attacks this turn.'
            -- TODO
        ),
        _defineMisery(
            'Confusion', 0,
            'Move Pandora up to 1 space.',
            UM.Effects:MoveFighters(
                UM.Select:Fighters():Named('Pandora'):Build(),
                UM.Number:UpTo(1), false
            )
        ),
        _defineMisery(
            'Woe', 0,
            'Summon a Kakodaemon in Pandora\'s Zone.',
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions:CountGte(defeatedFighterSelector():Build(), 1),
                    UM.Conditions:CountGte(nodeSelector():Build(), 1)
                ),
                UM.Effects:ReviveAndSummon(
                    defeatedFighterSelector():BuildFirst(),
                    nodeSelector():BuildOne()
                )
            )
        ),
        _defineMisery(
            'Hope', 0,
            'Look at the top card of Pandora\'s Box before deciding whether to reveal it.'
            -- TODO
        ),
        _defineMisery(
            'Misfortune', 2, ''
        ),
        _defineMisery(
            'Greed', 1,
            'Draw 1 card.',
            UM.Effects:Draw(
                UM.Select:Players():You():Build(),
                UM.Number:Static(1)
            )
        )
    }
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, open Pandora\'s Box.',
            UM.Effects.CharacterSpecific:OpenPandorasBox()
        )
    :Build()
end

function UM.Effects.CharacterSpecific:OpenPandorasBox()
    return function (args)
        
    end
end
