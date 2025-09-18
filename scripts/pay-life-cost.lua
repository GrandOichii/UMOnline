function _Create(text, children, data)
    assert(#children == 0, 'Expected children count of <pay-life-cost> to be 0')
    assert(#data == 2, 'Expected data count of <pay-life-cost> to be 2')
    return 'Cost.PayLife('..data[2]..')'
end