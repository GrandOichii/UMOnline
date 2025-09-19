function _Create(text, children)
    return string.format(
        'UM.S.Fighters()\n%s\n:Build()',
        children[1]
    )
end