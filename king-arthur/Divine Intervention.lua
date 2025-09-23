function _Create()
return UM:Card()
:AfterCombat(
'After combat: Move King Arthur up to 5 spaces.',
UM.Effects:MoveFighters(
UM.S:Fighters()
:Named('King Arthur')
:Single()
:Build(),
UM:UpTo(5)
)
)
:Build()
end