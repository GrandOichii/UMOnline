function _Create()
return UM:Card()
:AfterCombat(
'After combat: If you won the combat, draw 2 cards.',
UM:If(
UM.Conditional:CombatWonBy(
UM.Players:EffectOwner()
),
UM.Effects:Draw(
UM:Static(2)
)
)
)
:Build()
end