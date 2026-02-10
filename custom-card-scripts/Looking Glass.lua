function _Create()
    return UM.Build:Card()
        :AfterCombat(
            'After combat: Choose 2 different effects:\n- draw 2 cards\n- Alice recovers 2 health\n- place Alice in any other space',
            UM.Effects:Choose(
                2, false,
                UM:ChoiceEffect(
                    'Draw 2 cards',
                    UM.Effects:Draw(
                        UM.Select:Players()
                            :You()
                            :Build(),
                        UM.Number:Static(2)
                    )
                ),
                UM:ChoiceEffect(
                    'Alice recovers 3 health',
                    UM.Effects:Recover(
                        UM.Select:Fighters()
                            :Named('Alice')
                            :Build(),
                        UM.Number:Static(3)
                    )
                ),
                UM:ChoiceEffect(
                    'Place Alice in any other space',
                    UM.Effects:PlaceFighter(
                        UM.Fighter:Named('Alice'),
                        UM.Select:Nodes()
                            :Empty()
                            :BuildOne()
                    )
                )
            )
        )
    :Build()
end