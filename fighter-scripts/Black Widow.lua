function _Create()
    return UM.Build:Fighter()
    :Build()
end

function UM.Effects.CharacterSpecific:AcquireNewMission()
    return function (args)
        -- TODO reveal cards from the top
        -- TODO of your deck one at a time until you reveal a new mission card.
        -- TODO Add that mission card to your hand. Then, shuffle the other
        -- TODO cards you revealed back into your deck.
    end
end

