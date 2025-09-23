function _Create()
return UM:Card()
:AfterCombat(
'After combat: Move your fighter up to 4 spaces.',
UM.Effects:MoveFighters(
UM.S:Fighters()
:OwnedBy(UM.Players:EffectOwner())
:Single()
:Build(),
UM:UpTo(4)
)
)
:Build()
end