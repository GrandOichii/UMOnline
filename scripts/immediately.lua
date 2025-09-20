function _Create(text, children)
    return string.format(
        ':Immediately(\n\'%s\',\n%s\n)',
        text:gsub("'", "\\'"),
        children[1]
    )
end