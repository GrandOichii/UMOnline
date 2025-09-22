function _Create()
return UM:Card()
:AfterCombat(
'After combat: Draw 1 card. If you won the combat, draw 2 cards instead.',
UM:IfInstead(
UM.Conditional:CombatWonBy(
UM.Players:EffectOwner()
),
UM.Effects:Draw(
UM:Static(2)
),
UM.Effects:Draw(
UM:Static(1)
)
)
)
:Build()
end