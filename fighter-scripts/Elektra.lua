-- The first time Elektra would be defeated, remove her and all Hand from the board. She is not defeated. At the start of your next turn, Resurrect her. (Ignore effects with the RESURRECTED symbol.)
-- When Elektra Resurrects: Flip your health dial. Shuffle your discard pile into your deck.
-- Place Elektra and all Hand back onto the board with each fighter in a different zone. (You must resolve effects with the RESURRECTED symbol.)

-- TODO very low-level

function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfTheGame(
            'You shouldn\'t see this text',
            function (args)
                SetPlayerStringAttribute(args.owner, 'RESURRECTED', 'N')
            end
        )
        :OnFighterDefeatUngated(
            'The first time Elektra would be defeated, remove her and all Hand from the board. She is not defeated. At the start of your next turn, Resurrect her.',
            UM.Select:Fighters():Named('Elektra'):BuildPredicate(),
            UM.Effects:If(
                UM.Conditions:Not(
                    UM.Conditions.CharacterSpecific:IsElektraResurrected()
                ),
                function (args)
                    --  remove her and all Hand from the board. She is not defeated. At the start of your next turn, Resurrect her. (Ignore effects with the RESURRECTED symbol.)
                    SetPlayerStringAttribute(args.owner, 'RESURRECTED', 'Y')

                    local sidekicks = UM.Select:Fighters():Named('The Hand'):Build()(args)
                    for _, fighter in ipairs(sidekicks) do
                        RemoveFighterFromBoard(fighter)
                    end

                    local originalFLN = args.fighter.FormattedLogName
                    LogPublic(originalFLN..' wont stay dead for long...')

                    SetFighterName(args.fighter, 'Elektra Resurrected')
                    SetHealth(args.fighter, 9)
                    SetMaxHealth(args.fighter, 9)


                    AddAtTheStartOfNextTurnEffect(
                        args.owner,
                        args.fighter,
                        {
                            text = 'At the start of your next turn, Resurrect Elektra',
                            cond = function (_)
                                return true
                            end,
                            effects = {
                                [1] = function (nargs)
                                    LogPublic(originalFLN..' resurrects!')

                                    ShuffleDiscardIntoDeck(nargs.owner)

                                    local offBoardFighters = {}
                                    offBoardFighters[#offBoardFighters+1] = UM.Fighter:Named('Elektra Resurrected')(nargs)

                                    for _, fighter in ipairs(UM.Select:Fighters():Named('The Hand'):Build()(nargs)) do
                                        offBoardFighters[#offBoardFighters+1] = fighter
                                    end
                                    local pickedZones = {}
                                    for _, fighter in ipairs(offBoardFighters) do
                                        FullyRecoverHealth(fighter)

                                        local nodeSelect = UM.Select:Nodes()
                                            :Empty()
                                        for _, zone in ipairs(pickedZones) do
                                            nodeSelect = nodeSelect:NotInZone(zone)
                                        end

                                        if #nodeSelect:Build()(nargs) == 0 then
                                            nodeSelect = UM.Select:Nodes():Empty()
                                        end

                                        local targetNode = nodeSelect:BuildOne()(args, 'Choose where to place '..fighter.FormattedLogName)

                                        UM.Effects:PlaceFighter(
                                            function (_)
                                                return fighter
                                            end,
                                            function (_)
                                                return {
                                                    [1] = targetNode
                                                }
                                            end
                                        )(nargs)

                                        local zones = GetFighterZones(fighter)
                                        for _, zone in ipairs(zones) do
                                            pickedZones[#pickedZones+1] = zone
                                        end
                                    end
                                end
                            }
                        }
                    )
                end
            )
        )
    :Build()
end

function UM.Conditions.CharacterSpecific:IsElektraResurrected()
    return function (args)
        return GetPlayerStringAttribute(args.owner, 'RESURRECTED') == 'Y'
    end
end