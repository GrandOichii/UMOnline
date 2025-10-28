
function _Create()
    return UM.Build:Fighter()
        :OnAttack(
            'When King Arthur attacks, you may BOOST that attack. Play the BOOST card, face down, along with your attack card. (If your opponent cancels the effects on your attack card, the BOOST is discarded without effect)',
            UM.Select:Fighters():AllYour():Named('King Arthur'):FighterPredicate(),
            UM.Effects:AllowBoost(
                UM.Number:Static(1),
                true
            )
        )
    :Build()
end
