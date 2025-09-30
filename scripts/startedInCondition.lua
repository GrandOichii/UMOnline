function _Create(text, children)
    return string.format(
        'UM:IfInstead(\n%s,\n%s,\n%s\n)',
        children[2],
        children[3],
        children[1]
    )
end
