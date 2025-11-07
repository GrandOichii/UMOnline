
function _Create()
    return UM.Build:Fighter()
        :DeclareToken(
            'Insight',
            UM.Build:Token()
                :Amount(5)
                :Build()
        )
        :AfterMove(
            'After Dr. Sattler or Dr. Malcolm move, place an insight token in their new space.',
            UM.Select:Fighters():Named('Dr. Sattler', 'Dr. Malcolm'):BuildPredicate(),
            UM.Effects:PlaceTokens(
                'Insight',
                UM.Select:Nodes():WithFighter(UM.Fighter:Moving()):Build()
            )
        )
    :Build()
end