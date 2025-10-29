
function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may place a trap in an unoccupied node in Robert Muldoon\'s zone.',
            UM.Effects:If(
                UM.Conditions:TokensLeft('Trap'),
                UM.Effects:PlaceToken(
                    'Trap',
                    UM.Select:Nodes()
                        :Unoccupied()
                        :InZoneOfFighter(
                            UM.Select:Fighters()
                                :Named('Robert Muldoon')
                                :BuildOne()
                        )
                        :WithNoToken('Trap')
                        :Single()
                        :Build()
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
                    'When a fighter steps onto a Trap, deal 1 damage to them. Remove the trap from the board',
                    -- UM.Select:Fighters():Opposing():FighterPredicate(),
                    UM.Select:Fighters():FighterPredicate(),
                    UM.Effects:CancelCurrentMovement(),
                    UM.Effects:DealDamage(
                        UM.Select:Fighters():MovingFighter():Build(),
                        UM.Number:Static(1)
                    ),
                    UM.Effects:RemoveToken(
                        UM.Select:Tokens():Only(UM.Token:Source()):Build()
                    )
                )
                -- TODO add OnStep for friendly fighters
            :Build()
        )
    :Build()
end
