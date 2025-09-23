function _Create(text, children)
    return string.format(
        'UM.Effects:Draw(\n%s\n)',
        children[1]
    )
end