function _Create()
return UM:Card()
:AfterCombat(
'After combat: Your opponent discards 1 card.',
UM.Effects:Discard(
UM.Players:Opponent(),
UM:Static(1),
false
)
)
:Build()
end