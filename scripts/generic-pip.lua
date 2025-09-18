function _Create(text, children, data)
    assert(#children == 0, 'Expected children count of <generic-pip> to be 0')
    assert(#data == 1, 'Expected data count of <generic-pip> to be 1')
    return 'Mana.Generic('..data[1]..')'
end