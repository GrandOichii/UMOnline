delete from parsers;
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    1,   -- id
    'm:main', -- name
    1,   -- ptype
    '^(.*)$', -- pattern
    'function _Create(text, children, data) return string.format(''function _Create()\nreturn UM.Build:Card()\n%s\n:Build()\nend'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    1,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    2,   -- id
    's:mainSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    1,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    3,   -- id
    'm:empty', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''--'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    2,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    4,   -- id
    'sp:mainSplitter', -- name
    3,   -- ptype
    '
', -- pattern
    'function _Create(text, children)
    local result = ''''
    for i, child in ipairs(children) do
        if child ~= '''' then
            if i ~= 1 then
                result = result..'',\n''
            end
            result = result..child
        end
    end
    return result
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    2,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    5,   -- id
    's:abilitySelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    4,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    6,   -- id
    'm:duringCombat', -- name
    1,   -- ptype
    '^During combat: (.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':DuringCombat(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    5,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    7,   -- id
    'lineSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    6,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    8,   -- id
    'm:afterCombat', -- name
    1,   -- ptype
    '^After combat: (.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':AfterCombat(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    5,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    9,   -- id
    'lineSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    8,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    10,   -- id
    'm:immediately', -- name
    1,   -- ptype
    '^Immediately: (.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':Immediately(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    5,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    11,   -- id
    'lineSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    10,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    12,   -- id
    'm:scheme', -- name
    1,   -- ptype
    '^(.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':Effect(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    5,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    13,   -- id
    'lineSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    12,   -- parent_id
    1    -- is_ref
);





INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    14,   -- id
    's:lineSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    15,   -- id
    'm-effects', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    14,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    16,   -- id
    'm:2+sentence', -- name
    1,   -- ptype
    '^(.+)\. ([^\.]+)$', -- pattern
    'function _Create(text, children, data) return children[1]..'',\n''..children[2] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    14,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    17,   -- id
    'lineSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    16,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    18,   -- id
    'simpleEffects', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    16,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    19,   -- id
    'simpleEffects', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    14,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    20,   -- id
    's:m-effects', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    21,   -- id
    'controlFlow', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    20,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    22,   -- id
    'multiSentenceEffects', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    20,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    23,   -- id
    'complexCharacterSpecific', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    20,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    24,   -- id
    's:simpleEffects', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    25,   -- id
    'handManipulationSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    24,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    26,   -- id
    'm:actionGain', -- name
    1,   -- ptype
    '^[G|g]ain (.+) actions?$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:GainActions(%s)'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    24,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    27,   -- id
    'm:actionGainAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    26,   -- parent_id
    0    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    28,   -- id
    'fighterManipulationSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    24,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    29,   -- id
    'todoSortMeSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    24,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    30,   -- id
    'simpleCharacterSpecific', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    24,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    31,   -- id
    's:controlFlow', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    32,   -- id
    'm:ifInsteadMatcher', -- name
    1,   -- ptype
    '^(.+\. If .+, .+ instead)$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    31,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    33,   -- id
    's:ifInsteadSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    32,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    34,   -- id
    'm:ifInsteadTotal', -- name
    1,   -- ptype
    '^(.+)\. If (.+), (.+) instead$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    33,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    35,   -- id
    'simpleEffects', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    34,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    36,   -- id
    's:ifInsteadTotalCondition', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    34,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    37,   -- id
    'simpleEffects', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    34,   -- parent_id
    1    -- is_ref
);



INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    38,   -- id
    'm:if', -- name
    1,   -- ptype
    '^(If [^\.]+, [^\.]+)$', -- pattern
    'function _Create(text, children, data) return ''TODO'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    31,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    39,   -- id
    's:ifSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    38,   -- parent_id
    0    -- is_ref
);



INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    40,   -- id
    's:multiSentenceEffects', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    41,   -- id
    'm:doSomethingThenAddBoostValue', -- name
    1,   -- ptype
    '^(.+)\. Add its BOOST value to (.+)$', -- pattern
    'function _Create(text, children, data) return ''TODO'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    40,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    42,   -- id
    's:doSomethingThenAddBoostValueFrom', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    41,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    43,   -- id
    's:doSomethingThenAddBoostValueTo', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    41,   -- parent_id
    0    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    44,   -- id
    's:msFighterManipulation', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    40,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    45,   -- id
    's:msFighterPlacement', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    44,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    46,   -- id
    'm:chooseSpaceThenPlaceThere', -- name
    1,   -- ptype
    '^Choose (.+)\. Place (.+) in that space$', -- pattern
    'function _Create(text, children, data) return ''TODO'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    45,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    47,   -- id
    's:chooseSpaceThenPlaceThereWhere', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    46,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    48,   -- id
    's:chooseSpaceThenPlaceThereWhat', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    46,   -- parent_id
    0    -- is_ref
);



INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    49,   -- id
    's:msFighterMovement', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    44,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    50,   -- id
    'm:moveMayMoveThroughOpposing', -- name
    1,   -- ptype
    '^[M|m]ove (.+) up to (.+) spaces\. .+ through spaces containing opposing fighters$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:MoveFighters(\n%s,\nUM.Number:UpTo(%s),\ntrue\n)'', children[1], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    49,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    51,   -- id
    'multipleFighters', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    50,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    52,   -- id
    'm:moveMayMoveThroughOpposingAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    50,   -- parent_id
    0    -- is_ref
);





INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    53,   -- id
    's:complexCharacterSpecific', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    54,   -- id
    's:handManipulationSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    55,   -- id
    'm:draw', -- name
    1,   -- ptype
    '^(.+? )?(may )?[D|d]raws? (.+) cards?$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:Draw(\n%s, \n%s, \n%s\n)'', children[1], children[3], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    54,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    56,   -- id
    'multiplePlayers', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    55,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    57,   -- id
    's:drawIsOptional', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    55,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    58,   -- id
    'm:drawIsOptionalTrue', -- name
    1,   -- ptype
    '^may $', -- pattern
    'function _Create(text, children, data) return ''true'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    57,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    59,   -- id
    'm:drawIsOptionalFalse', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''false'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    57,   -- parent_id
    0    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    60,   -- id
    'numericSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    55,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    61,   -- id
    'm:discard', -- name
    1,   -- ptype
    '^(.+? )?[D|d]iscards? (.+?)( random)? cards?$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:Discard(\n%s, \n%s, \n%s\n)'', children[1], children[2], children[3]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    54,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    62,   -- id
    'multiplePlayers', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    61,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    63,   -- id
    'numericSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    61,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    64,   -- id
    's:discardIsRandom', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    61,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    65,   -- id
    'm:discardIsRandomFalse', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''false'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    64,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    66,   -- id
    'm:discardIsRandomTrue', -- name
    1,   -- ptype
    '^ random$', -- pattern
    'function _Create(text, children, data) return ''true'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    64,   -- parent_id
    0    -- is_ref
);




INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    67,   -- id
    'm:multiplePlayers', -- name
    1,   -- ptype
    '^(.*)$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Select:Players()\n%s\n:Build()'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    68,   -- id
    's:multiplePlayersSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    67,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    69,   -- id
    'm:multiplePlayersEffectOwner', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return '':You()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    68,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    70,   -- id
    'm:multiplePlayersYou', -- name
    1,   -- ptype
    '^[Y|y]ou $', -- pattern
    'function _Create(text, children, data) return '':You()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    68,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    71,   -- id
    'm:multiplePlayersYourOpponent', -- name
    1,   -- ptype
    '^[Y|y]our opponent $', -- pattern
    'function _Create(text, children, data) return '':YourOpponent()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    68,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    72,   -- id
    'm:multiplePlayersEach', -- name
    1,   -- ptype
    '^[E|e]ach (.+)$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    68,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    73,   -- id
    's:multiplePlayersEachSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    72,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    74,   -- id
    'm:multiplePlayersEach', -- name
    1,   -- ptype
    '^opponent $', -- pattern
    'function _Create(text, children, data) return '':Opponents()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    73,   -- parent_id
    0    -- is_ref
);





INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    75,   -- id
    's:numericSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    76,   -- id
    'm:numericStatic', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Number:Static(%s)'', text) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    75,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    77,   -- id
    'm:numericStaticA', -- name
    1,   -- ptype
    '^a$', -- pattern
    'function _Create(text, children, data) return ''UM.Number:Static(1)'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    75,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    78,   -- id
    'm:numericUpTo', -- name
    1,   -- ptype
    '^up to ([0-9])$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Number:UpTo(%s)'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    75,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    79,   -- id
    'm:numericUpToNumber', -- name
    1,   -- ptype
    '^.+$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    78,   -- parent_id
    0    -- is_ref
);



INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    80,   -- id
    's:fighterManipulationSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    81,   -- id
    'fighterMovementSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    80,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    82,   -- id
    'healthManipulationSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    80,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    83,   -- id
    's:fighterMovementSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    84,   -- id
    'm:move', -- name
    1,   -- ptype
    '^[M|m]oves? (.+) up to (.+) spaces$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:MoveFighters(\n%s,\nUM.Number:UpTo(%s),\nfalse\n)'', children[1], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    83,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    85,   -- id
    'multipleFighters', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    84,   -- parent_id
    1    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    86,   -- id
    'm:moveAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    84,   -- parent_id
    0    -- is_ref
);



INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    87,   -- id
    'm:multipleFighters', -- name
    1,   -- ptype
    '^(.+)$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Select:Fighters()\n%s\n:Build()'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    88,   -- id
    's:multipleFightersSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    87,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    89,   -- id
    'm:multipleFightersYourFighter', -- name
    1,   -- ptype
    '^[Y|y]our fighter$', -- pattern
    'function _Create(text, children, data) return '':YourFighter()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    90,   -- id
    'm:multipleFightersOpposingFighter', -- name
    1,   -- ptype
    '^[T|t]he opposing fighter$', -- pattern
    'function _Create(text, children, data) return '':Opposing()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    91,   -- id
    'm:multipleFightersEach', -- name
    1,   -- ptype
    '^[E|e]ach (.+)$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    92,   -- id
    'eachFighterSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    91,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    93,   -- id
    'm:multipleFightersYourOtherFighter', -- name
    1,   -- ptype
    '^[Y|y]our other fighter$', -- pattern
    'function _Create(text, children, data) return '':OtherThanSource()\n:Your()\n:Single()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    94,   -- id
    'm:multipleFightersAnyFighter', -- name
    1,   -- ptype
    '^[A|a]ny fighter$', -- pattern
    'function _Create(text, children, data) return ''--'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    95,   -- id
    'm:multipleFightersAllOpposingFighters', -- name
    1,   -- ptype
    '^[A|a]ll opposing fighters$', -- pattern
    'function _Create(text, children, data) return '':Opposing()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    96,   -- id
    'm:anySingleFighter', -- name
    1,   -- ptype
    '^(?:any 1|1|an|a) (.+)$', -- pattern
    'function _Create(text, children, data) return children[1]..''\n:Single()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    97,   -- id
    'eachFighterSelector', -- name
    3,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    96,   -- parent_id
    1    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    98,   -- id
    'm:namedFighters', -- name
    1,   -- ptype
    '^(.+)$', -- pattern
    'function _Create(text, children, data) return string.format('':Named(\''%s\''):Single()'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    88,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    99,   -- id
    'fighterNames', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    98,   -- parent_id
    1    -- is_ref
);



INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    100,   -- id
    'm:fighterNames', -- name
    1,   -- ptype
    '^(?:Harpy|Wolf|squirrel|Actor|Alice|Sinbad|Bigfoot|Dr{DOTSPACE}Jekyll|King Arthur|Faith|Holmes|Little Red|Ghost Rider|Daredevil|Bullseye|Elektra|Black Widow|Golden Bat|Ciri|Geralt|Buffy|Tesla|Raptors|InGen Workers|Shakespeare|Ihuarraquax|She-Hulk|the Jackalope|Shuri|The Genie|Squirrel Girl|Yennenga)$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    101,   -- id
    's:todoSortMeSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    102,   -- id
    'm:cancelAllEffectsOnOpponentsCard', -- name
    1,   -- ptype
    '^Cancel all effects on your opponent's card$', -- pattern
    'function _Create(text, children, data) return ''UM.Effects:CancelAllEffectsOnOpponentsCard()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    101,   -- parent_id
    0    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    103,   -- id
    's:simpleCharacterSpecific', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    104,   -- id
    'm:changeSize', -- name
    1,   -- ptype
    '^[C|c]hange size$', -- pattern
    'function _Create(text, children, data) return ''UM.Effects.CharacterSpecific:ChangeSize()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    103,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    105,   -- id
    'm:acquireNewMission', -- name
    1,   -- ptype
    '^Acquire a new mission$', -- pattern
    'function _Create(text, children, data) return ''UM.Effects.CharacterSpecific:AcquireNewMission()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    103,   -- parent_id
    0    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    106,   -- id
    's:healthManipulationSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    107,   -- id
    'm:recover', -- name
    1,   -- ptype
    '^(.+)[R|r]ecovers? (.+) health$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:Recover(\n%s,\n%s\n)'', children[1], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    106,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    108,   -- id
    's:recoverTargetSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    107,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    109,   -- id
    'm:recoverTargetFighter', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''UM.Select:Fighters():YourFighter():Build()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    108,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    110,   -- id
    'm:recoverTargetMultipleFighters', -- name
    1,   -- ptype
    '^(.+) $', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    108,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    111,   -- id
    'multipleFighters', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    110,   -- parent_id
    1    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    112,   -- id
    'm:recoverAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    107,   -- parent_id
    0    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    113,   -- id
    'm:dealDamage', -- name
    1,   -- ptype
    '^[D|d]eal (.+) damage to (.+)$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:DealDamage(\n%s,\nUM.Number:Static(%s)\n)'', children[2], children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    106,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    114,   -- id
    'm:dealDamageAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    113,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    115,   -- id
    'multipleFighters', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    113,   -- parent_id
    1    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    116,   -- id
    's:eachFighterSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    117,   -- id
    'm:eachFighter', -- name
    1,   -- ptype
    '^(.+ )?fighters?$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    116,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    118,   -- id
    's:allFightersSelector', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    117,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    119,   -- id
    'm:allFightersEmpty', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''--'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    118,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    120,   -- id
    'm:allYourFighters', -- name
    1,   -- ptype
    '^of your $', -- pattern
    'function _Create(text, children, data) return '':Your()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    118,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    121,   -- id
    'm:eachAdjacent', -- name
    1,   -- ptype
    '^adjacent $', -- pattern
    'function _Create(text, children, data) return '':Adjacent()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    118,   -- parent_id
    0    -- is_ref
);



INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    122,   -- id
    'm:eachFighterInCombat', -- name
    1,   -- ptype
    '^fighter in the combat$', -- pattern
    'function _Create(text, children, data) return '':InCombat()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    116,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    123,   -- id
    'm:eachNamedFighter', -- name
    1,   -- ptype
    '^(of your )?(.+)$', -- pattern
    'function _Create(text, children, data) return string.format(''%s\n:Named(\''%s\'')'', children[1], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    116,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    124,   -- id
    's:eachNamedFighterOwned', -- name
    2,   -- ptype
    '', -- pattern
    'function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '''' then
            return child
        end
    end
end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    123,   -- parent_id
    0    -- is_ref
);
INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    125,   -- id
    'm:eachNamedFighterOwnedFalse', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''--'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    124,   -- parent_id
    0    -- is_ref
);

INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    126,   -- id
    'm:eachNamedFighterOwnedTrue', -- name
    1,   -- ptype
    '^of your $', -- pattern
    'function _Create(text, children, data) return '':Your()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    124,   -- parent_id
    0    -- is_ref
);


INSERT INTO parsers(
    id,
    name,
    ptype,
    pattern,
    script,
    project_name,
    description,
    is_template,
    is_root,
    parent_id,
    is_ref
) VALUES (
    127,   -- id
    'fighterNames', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    123,   -- parent_id
    1    -- is_ref
);


