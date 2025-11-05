function _Create()
    local fighterSelector = function ()
        return UM.Select:Fighters()
            :OpposingTo(UM.Player:EffectOwner())
            :InSameZoneAs(UM.Fighter:Source())
    end

    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may deal 1 damage to an opposing fighter in Medusa\'s zone.',
            UM.Effects:If(
                UM.Conditions:CountGte(fighterSelector():Build(), 1),
                UM.Effects:Optional(
                    'Deal 1 damage to an a fighter in the same zone as Medusa',
                    UM.Effects:DealDamage(
                        fighterSelector():Single():Build(),
                        UM.Number:Static(1)
                    )
                )
            )
        )
    :Build()
end
