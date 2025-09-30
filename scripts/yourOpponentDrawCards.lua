function _Create(text, children)
    return string.format(
        'UM.Effects:Draw(\n%s,\nUM.Player:Opponent()\n)',
        children[1]
    )
end