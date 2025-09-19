function _Create(text, children)
    return string.format(
        'UM.Effects:DealDamage(\n%s,\n%s\n)',
        children[1],
        children[2]
    )
end