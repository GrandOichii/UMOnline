function _Create(text, children)
    return string.format(
        'UM.Effects:GainActions(\n%s\n)',
        children[1]
    )
end