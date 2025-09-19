function _Create(text, children)
    return string.format(
        'UM.Effects:MoveFighter(\n%s,\n%s\n)',
        children[1],
        children[2]
    )
end