function _Create(text, children)
    return string.format(
        'UM.Effects:Draw(\n%s,\nUM.Players:Opponent()\n)',
        children[1]
    )
end