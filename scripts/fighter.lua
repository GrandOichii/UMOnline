function _Create(text, children)
    local format = 'UM.S:Fighters()\n%s\n:Build()'
    for _, child in ipairs(children) do
        -- return child
        if child == 'EMPTY_STRING' then
            return string.format(format, '')
        end
        if child ~= '' then
            return string.format(format, child)
        end
    end
    return 'ERR_NO_VALID_CHILDREN'
end