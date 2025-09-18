function _Create(text, children)
    local result = '.Effect(\n'
    for _, child in ipairs(children) do
        if child ~= '' then
            return result..child..'\n)'
        end
    end
end