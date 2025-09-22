function _Create(text, children)
    return string.format(
        ':AfterCombat(\n\'%s\',\n%s\n)',
        text:gsub("'", "\\'"):gsub('{DOTSPACE}', '. '),
        children[1]
    )
end