-- Your cards go into your cauldron instead of your discard pile.
-- After you attack, you may cast one spell that you have the ingredients for. If you do, discard all the cards in your cauldron.

-- TODO there are multiple wayward sisters, each one will try to run this script :)

function _Create()
    return UM.Build:Fighter()
        :DefineCardZone(
            'CAULDRON',
            UM.Build:CardZone()
                :Build()
        )
        :RedirectCardZoneChange(
            'WAYWARD_SISTERS_CARD_REDIRECTOR', -- id, to prevent multiple card zone change redirectors
            'Your cards go into your cauldron instead of your discard pile.',
            function (args, zoneChange)
                local card = zoneChange.Card
                -- if isn't card owner
                if card.Owner ~= args.owner then
                    return false
                end
                -- if isn't going to discard
                local targetZone = zoneChange.TargetZone
                if targetZone:GetName() ~= 'DISCARD' then
                    return false
                end

                -- check that is not discarding a card from the cauldron
                local fromZone = zoneChange.FromZone
                if fromZone:GetName() == 'CAULDRON' then
                    return
                end

                local zone = GetZone(args.owner, 'CAULDRON')
                ChangeTargetZone(zoneChange, zone)
            end
        )
        -- :AfterAttack(
        --     'After you attack, you may cast one spell that you have the ingredients for. If you do, discard all the cards in your cauldron.',
        --     UM.Select:Fighters():Named('Wayward Sister'):BuildPredicate(),
        --     UM.Effects.CharacterSpecific:PromptCastCauldronSpell()
        -- )
    :Build()
end