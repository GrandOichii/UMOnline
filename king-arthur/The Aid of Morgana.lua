function _Create()
return UM:Card()
:AfterCombat(
'After combat: Draw 2 cards.',
UM.Effects:Draw(
UM:Static(2)
)
)
:Build()
end