function _Create(text, children)
    local result = ''
    for _, child in ipairs(children) do
    result = result..string.format('.Cost(\n%s\n)\n', child)
    end
    return result
end