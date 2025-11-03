
function _Create()
    local nodeFilter = function ()
        return UM.Select:Nodes()
        :Unoccupied()
        :InZoneOfFighter(UM.Fighter:Named('Robert Muldoon'))
        :WithNoToken('Trap')
    end

    local onStepEffect = {
        [1] = UM.Effects:CancelCurrentMovement(),
        [2] = UM.Effects:DealDamage(
            UM.Select:Fighters():MovingFighter():Build(),
            UM.Number:Static(1)
        ),
        [3] = UM.Effects:RemoveTokens(
            UM.Select:Tokens():Only(UM.Token:Source()):Build()
        )
    }

    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may place a trap in an unoccupied node in Robert Muldoon\'s zone.',
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions:TokensLeft('Trap'),
                    UM.Conditions:CountGte(nodeFilter():Build(), 1)
                ),
                UM.Effects:Optional(
                    'Place a trap token?',
                    UM.Effects:PlaceTokens(
                        'Trap',
                        nodeFilter()
                            :Single()
                            :Build()
                    )
                )
            )
        )
        :DeclareToken(
            'Trap',
            UM.Build:Token()
                :Amount(8)
                :WhenReturnedToBox(
                    'Whenever one of your traps is returned to the box, draw a card.',
                    UM.Effects:Draw(
                        UM.Select:Players():You():Build(),
                        UM.Number:Static(1),
                        false
                    )
                )
                :OnStep(
                    'When an opposing fighter steps onto a Trap, deal 1 damage to them and stop their movement. Remove the trap from the board',
                    UM.Select:Fighters():Opposing():BuildPredicate(),
                    table.unpack(onStepEffect)
                )
                :OnStep(
                    'When a friendly fighter steps onto a Trap, you may deal 1 damage to them and stop their movement. If you do, Remove the trap from the board',
                    UM.Select:Fighters():Friendly():BuildPredicate(),
                    UM.Effects:Optional(
                        'Trigger trap on friendly fighter?',
                        table.unpack(onStepEffect)
                    )
                )
            :Build()
        )
    :Build()
end
