function _Create(text, children)
    for _, child in ipairs(children) do
        if child == 'EMPTY_STRING' then
            return ''
        end
        if child ~= '' then
            return string.format('%s\n:Single()', child)
        end
    end
end
