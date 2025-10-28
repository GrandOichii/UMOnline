-- 
-- Whenever one of your traps is returned to the box, draw a card.

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may place a trap.',
            UM.Effects:PlaceToken(
                'Trap',
                UM.Select:Nodes():WithNoToken('Trap'):Build()
            )
        )
        :DeclareToken(
            UM.Build:Token()
                :Amount(8)
                :WhenReturnedToBox(
                    UM.Effects:Draw(
                        UM.Select:Players():You():Build(),
                        UM.Number:Static(1),
                        false
                    )
                )
                :OnStep(
                    UM.Select:Fighters():OpposingTo(UM.Player:EffectOwner()):FighterPredicate(),
                    UM.Effects:CancelCurrentMovement(),
                    UM.Effects:DealDamage(
                        UM.Select:Fighters():MovingFighter():Build(),
                        UM.Number:Static(1)
                    ),
                    UM.Effects:RemoveToken(
                        UM.Select:Tokens():Only(UM.Token:Source()):Build()
                    )
                )
            :Build()
        )
    :Build()
end
