function _Create()
return UM:Card()
:AfterCombat(
'After combat: Move each Harpy up to 3 spaces.',
UM.Effects:MoveFighters(
UM.S:Fighters()
:Named('Harpy')
:Build(),
UM:UpTo(3)
)
)
:Build()
end