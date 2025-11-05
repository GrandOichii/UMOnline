
function _Create()

    return UM.Build:Fighter()
        :OnDamage(
            'When Tara is dealt damage, Willow becomes Dark Willow',
            UM.Effects.CharacterSpecific:ToggleDarkWillow(true)
        )
    :Build()
end