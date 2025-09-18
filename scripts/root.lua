function _Create(text, children)
    local script = 'function _Create()\nreturn UM.Card()\n%s\n:Build()\nend'
    -- local script = '%s'
    return string.format(script, children[1])
end