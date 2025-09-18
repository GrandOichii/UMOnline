function _Create(text, children, data)
    assert(#children == 0, 'Expected children count of <life-gain> to be 0')
    assert(#data == 2, 'Expected data count of <life-gain> to be 2')

    return 'Effect.GainLife('..data[2]..')'
end