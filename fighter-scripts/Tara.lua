
function _Create()

    return UM.Build:Fighter()
        :OnDamage(
            'When Tara is dealt damage, Willow becomes Dark Willow',
            UM.Select:Fighters():Named('Tara'):BuildPredicate(),
            UM.Effects.CharacterSpecific:ToggleDarkWillow(true)
        )
    :Build()
end