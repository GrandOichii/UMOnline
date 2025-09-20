function _Create(text, children)
    for _, child in ipairs(children) do
        -- print(child)
        -- return child
        if child == 'EMPTY_STRING' then
            return ''
        end
        if child ~= '' then
            return child
        end
    end
end