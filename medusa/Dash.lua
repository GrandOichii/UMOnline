function _Create()
return UM:Card()
:AfterCombat(
'After combat: Move your fighter up to 3 spaces.',
UM.Effects:MoveFighters(
UM.S:Fighters()
:OwnedBy(UM.Players:EffectOwner())
:Single()
:Build(),
UM:UpTo(3)
)
)
:Build()
end