function _Create(text, children)
    return string.format(
        ':AfterCombat(\n\'%s\',\n%s\n)',
        text,
        children[1]
    )
end