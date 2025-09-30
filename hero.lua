-- bloody mary
-- "At the start of your turn, if you have exactly 3 cards in hand, gain 1 action."

function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, if you have exactly 3 cards in hand, gain 1 action.',
            UM:If(
                UM.Conditional:HandCountEq(
                    UM.Player:EffectOwner(),
                    UM:Static(3)
                ),
                UM.Effects:GainActions(
                    UM:Static(1)
                )
            )
        )
    :Build()
end

-- Philippa

function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the end of your turn, you may draw until you have a hand of 4 cards.',
            UM:If(
                UM.Conditional:HandCountLt(
                    UM:Static(4)
                ),
                UM.Effects:Optional(
                    'Draw up to 4 cards?',
                    UM.Effects:DrawUntilHandCount(
                        UM:Static(4)
                    )
                )
            )
        )
    :Build()
end

-- Raphael
-- On each of your turns, the first time you lose combat, gain 1 action.
function _Create()
    return UM:Fighter()
        :OnCombatLoss(
            'On each of your turns, the first time you lose combat, gain 1 action.',
            UM.Trigger:IsFirstTime(
                UM.Effects:GainActions(UM:Static(1)),
                nil
            )
        )
    :Build()
end

-- The Genie
-- At the start of your turn, you may discard 1 card to gain 1 action.

function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may discard 1 card to gain 1 action.',
            UM:If(
                UM.Conditional:HandCountGte(
                    UM:Static(1)
                ),
                UM.Effects:Optional(
                    'Discard 1 card to gain 1 action?',
                    UM.Effects:Discard(
                        UM.S.Players():EffectOwner():Build(),
                        UM:Static(1)
                    ),
                    UM.Effects:GainActions(
                        UM:Static(1)
                    )
                )
            )
        )
    :Build()
end