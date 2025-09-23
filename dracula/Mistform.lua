function _Create()
return UM:Card()
:Effect(
'Place Dracula in any space.',
UM.Effects:PlaceFighter(
UM.S:Fighters()
:Named('Dracula')
:Single()
:Build(),
UM.S:Spaces()
--
:Build()
)
)
:Build()
end