function _Create()
    return UM.Build:Card()
        :Effect(
            'Draw 1 card, then choose an opponent. They may choose to discard 1 card. If they do not, draw 1 more card.',
            UM.Effects:Draw(
                UM.Select:Players():You():Build(),
                UM.Number:Static(1)
            ),
            function (args)
                local opp = UM.Select:Players():Opponents():BuildOne()(args, 'Choose a player')
                
                local choice = ChooseString(opp, { 'No', 'Yes'}, 'Discard a card?')

                if choice == 'No' then
                    return UM.Effects:Draw(
                        UM.Select:Players():You():Build(),
                        UM.Number:Static(1)
                    )(args)
                end

                return UM.Effects:Discard(
                    UM.Select:Players()
                        :RawOnly(opp)
                        :Build(),
                    1, false
                )(args)
            end
        )
    :Build()
end