function _Create(text, children)
    for _, child in ipairs(children) do
        -- print(child)
        if child ~= '' then
            return child
        end
    end
end