function _Create()
    return UM.Build:Fighter()
        :AtTheStartOfYourTurn(
            'At the end of your turn, if there are no opposing fighters in Bigfoot\'s zone, you may draw 1 card.',
            UM.Effects:If(
                UM.Conditions:Eq(
                    UM.Count:Fighters(
                        UM.Select:Fighters()
                            :OpposingTo(UM.Player:EffectOwner())
                            :InSameZoneAs(UM.Fighters:Source())
                            :Build()
                    ),
                    UM:Static(0)
                ),
                UM.Effects:Draw(
                    UM.Select:Players():You():Build(),
                    UM:Static(1),
                    true
                )
            )
        )
    :Build()
end
