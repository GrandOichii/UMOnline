
-- GADGETOLOGY
-- .
-- Whenever Jill Trent attacks, resolve the active gadget's effect.

-- Hypnoray Blaster
-- DURING COMBAT: If your card's printed value is lower than your opponent's, reveal the top card of your opponent's deck. Increase the value of your attack by the BOOST value of the revealed card.

-- Ultrabiotic Tonic
-- AFTER COMBAT: If your card's printed value is higher than your opponent's, Jill Trent Recovers 1 health.

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'Start the game with the Hypnoray Blaster.',
            UM.Effects.CharacterSpecific:ActivateGadget('blaster')
        )
        :AtTheStartOfYourTurn(
            'At the start of your turn, activate one of your gadgets.',
            function (args)
                local g1 = 'Hypnoray Blaster'
                local g2 = 'Ultrabiotic Tonic'

                local gadgetMap = {
                    [g1] = 'blaster',
                    [g2] = 'tonic',
                }

                local choice = ChooseString(args.owner, {
                    [1] = g1,
                    [2] = g2,
                }, 'Activate which gadget?')

                local target = gadgetMap[choice]
                UM.Effects.CharacterSpecific:ActivateGadget(target)(args)
            end
        )
        :DuringCombat(
            "During combat: If your card's printed value is lower than your opponent's, reveal the top card of your opponent's deck. Increase the value of your attack by the BOOST value of the revealed card.",
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions.CharacterSpecific:GadgetActive('blaster'),
                    UM.Conditions:CmpCombatPrintedValue(
                        UM.Player:EffectOwner(),
                        UM.Player:Opponent(),
                        UM.CmpOp:Lt()
                    ),
                    UM.Conditions:IsAttacker(
                        UM.Player:EffectOwner()
                    )
                ),
                UM.Effects:RevealTopCardOfDeck(
                    UM.Select:Players()
                        :InCombat()
                        :Build(),
                    'REVEALED'
                ),
                function (args)
                    -- TODO too low-level
                    local card = args.ctx['REVEALED'][1]
                    if card == nil then
                        return
                    end
                    local value = GetBoostValue(card)
                    AddToCardValueInCombat(args.owner, value)
                end
            )
        )
        :AfterCombat(
            "After combat: If your card's printed value is higher than your opponent's, Jill Trent Recovers 1 health.",
            UM.Effects:If(
                UM.Conditions:And(
                    UM.Conditions.CharacterSpecific:GadgetActive('tonic'),
                    UM.Conditions:CmpCombatPrintedValue(
                        UM.Player:EffectOwner(),
                        UM.Player:Opponent(),
                        UM.CmpOp:Gt()
                    ),
                    UM.Conditions:IsAttacker(
                        UM.Player:EffectOwner()
                    )
                ),
                UM.Effects:Recover(
                    UM.Select:Fighters()
                        :Named('Dr. Jill Trent')
                        :Build(),
                    UM.Number:Static(1)
                )
            )
        )
    :Build()
end

function UM.Conditions.CharacterSpecific:GadgetActive(gadget)
    return function (args)
        return gadget == GetPlayerStringAttribute(args.owner, 'GADGET')
    end
end

function UM.Effects.CharacterSpecific:ActivateGadget(gadget)
    return function (args)
        SetPlayerStringAttribute(args.owner, 'GADGET', gadget)
        LogPublic('Dr. Jill Trent activates '..gadget) -- TODO format name
    end
end