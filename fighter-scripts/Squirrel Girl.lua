-- 
-- Squirrels are small fighters. Do not start with any squirrels on the board.

-- Squirrel Girl’s sidekicks are squirrels. Do not place any squirrels at the start of the game. At the start of each of your turns, you may summon a squirrel following these summoning rules:
-- - If you have any squirrel tokens off the board, place one squirrel on an empty space adjacent to Squirrel Girl.
-- - If  all of your squirrel tokens are on the board, take one squirrel from the board and place it on an empty space adjacent to Squirrel Girl.

function _Create()
    local nodeSelect = function ()
        return UM.Select:Nodes()
            :CanFitSmallFighter()
            :AdjacentToFighters(
                UM.Select:Fighters()
                    :Named('Squirrel Girl')
                    :Build()
            )
    end
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, summon a squirrel in a space adjacent to Squirrel Girl.',
            {},
            UM.Effects:IfInstead(
                UM.Conditions:CountGte(
                    UM.Select:Fighters():Defeated():Named('Squirrel'):Build(),
                    1
                ),
                -- true
                UM.Effects:ReviveAndSummon(
                    UM.Select:Fighters():Defeated():Named('Squirrel'):BuildFirst(),
                    nodeSelect():BuildOne()
                ),
                -- false
                UM.Effects:PlaceFighter(
                    UM.Select:Fighters():Undefeated():Named('Squirrel'):BuildOne(),
                    nodeSelect():BuildOne()
                )
            )
        )
    :Build()
end