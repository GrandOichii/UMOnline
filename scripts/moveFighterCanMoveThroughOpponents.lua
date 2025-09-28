function _Create(text, children)
    return string.format(
        'UM.Effects:MoveFighters(\n%s,\n%s,\ntrue\n)',
        children[1],
        children[2]
    )
end