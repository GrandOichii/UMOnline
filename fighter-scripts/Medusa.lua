function _Create()
    return UM:Fighter()
        :AtTheStartOfYourTurn(
            'At the start of your turn, you may deal 1 damage to an opposing fighter in Medusa\'s zone.',
            UM:If(
                UM.Conditional:FightersCountGte(
                    UM.S:Fighters()
                        :OpposingTo(UM.Player:EffectOwner())
                        :InSameZoneAs(UM.Fighters:Source())
                        :Build(),
                    UM:Static(1)
                ),
                UM.Effects:Optional(
                    'Deal 1 damage to an a fighter in the same zone as Medusa?',
                    UM.Effects:DealDamage(
                        UM:Static(1),
                        UM.S:Fighters()
                            :OpposingTo(UM.Player:EffectOwner())
                            :InSameZoneAs(UM.Fighters:Source())
                            :Single()
                            :Build()
                    )
                )
            )
        )
    :Build()
end
