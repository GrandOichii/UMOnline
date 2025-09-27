function _Create()
return UM:Card()
:AfterCombat(
'After combat: Your opponent discards 1 card.',
UM.Effects:Discard(
UM.S:Players():OpposingTo(UM.Players:EffectOwner()):Single():Build(),
UM:Static(1),
false
)
)
:Build()
end