

function _Create()
    return UM.Build:Fighter()
        :ModCardValue(
            'Add +2 to the value of Annie\'s attacks if she has less health than the defender.',
            UM.Select:Fighters():Named('Annie Christmas'):BuildPredicate(),
            UM.Mod.Cards:AttackCards(UM.Number:Static(2)),
            UM.Conditions:CmpHealth(
                UM.Fighter:Source(),
                UM.Fighter:Defender(),
                UM.CmpOp:Lt()
            )
        )
    :Build()
end
