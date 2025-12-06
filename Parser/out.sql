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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    1,   -- id
    'root', -- name
    1,   -- ptype
    '^(.*)$', -- pattern
    'function _Create(text, children, data) return string.format(''function _Create()\nreturn UM.Build:Card()\n%s\n:Build()\nend'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    1,   -- is_root
    NULL,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    2,   -- id
    'line', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    3,   -- id
    'multiSentence', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    4,   -- id
    'simpleEffects', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    5,   -- id
    'controlFlow', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    6,   -- id
    'multiSentenceEffects', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    7,   -- id
    'complexCharacterSpecific', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    8,   -- id
    'handManipulation', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    9,   -- id
    'multiplePlayers', -- name
    1,   -- ptype
    '^(.*)$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Select:Players()\n%s\n:Build()'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    10,   -- id
    'numeric', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    11,   -- id
    'fighterManipulation', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    12,   -- id
    'fighterMovement', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    13,   -- id
    'multipleFighters', -- name
    1,   -- ptype
    '^(.+)$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Select:Fighters()\n%s\n:Build()'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    14,   -- id
    'fighterNames', -- name
    1,   -- ptype
    '^(?:Harpy|Wolf|squirrel|Actor|Alice|Sinbad|Bigfoot|Dr{{DOTSPACE}}Jekyll|King Arthur|Faith|Holmes|Little Red|Ghost Rider|Daredevil|Bullseye|Elektra|Black Widow|Golden Bat|Ciri|Geralt|Buffy|Tesla|Raptors|InGen Workers|Shakespeare|Ihuarraquax|She-Hulk|the Jackalope|Shuri|The Genie|Squirrel Girl|Yennenga)$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    1,   -- is_template
    0,   -- is_root
    NULL,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    15,   -- id
    'todoSortMe', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    16,   -- id
    'simpleCharacterSpecific', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    17,   -- id
    'healthManipulation', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    18,   -- id
    'eachFighterSelector', -- name
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    19,   -- id
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    20,   -- id
    'm:empty', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''--'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    19,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    21,   -- id
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
    19,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    22,   -- id
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
    21,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    23,   -- id
    'm:duringCombat', -- name
    1,   -- ptype
    '^During combat: (.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':DuringCombat(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    22,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    24,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    23,   -- parent_id
    2,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    25,   -- id
    'm:afterCombat', -- name
    1,   -- ptype
    '^After combat: (.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':AfterCombat(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    22,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    26,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    25,   -- parent_id
    2,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    27,   -- id
    'm:immediately', -- name
    1,   -- ptype
    '^Immediately: (.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':Immediately(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    22,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    28,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    27,   -- parent_id
    2,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    29,   -- id
    'm:scheme', -- name
    1,   -- ptype
    '^(.+?)\.?$', -- pattern
    'function _Create(text, children, data) return string.format('':Effect(\n\"%s\",\n%s\n)'', text, children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    22,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    30,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    29,   -- parent_id
    2,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    31,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    2,   -- parent_id
    3,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    32,   -- id
    'm:2+sentence', -- name
    1,   -- ptype
    '^(.+)\. ([^\.]+)$', -- pattern
    'function _Create(text, children, data) return children[1]..'',\n''..children[2] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    2,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    33,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    32,   -- parent_id
    2,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    34,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    32,   -- parent_id
    4,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    35,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    2,   -- parent_id
    4,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    36,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    3,   -- parent_id
    5,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    37,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    3,   -- parent_id
    6,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    38,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    3,   -- parent_id
    7,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    39,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    4,   -- parent_id
    8,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    40,   -- id
    'm:actionGain', -- name
    1,   -- ptype
    '^[G|g]ain (.+) actions?$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:GainActions(%s)'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    4,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    41,   -- id
    'm:actionGainAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    40,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    42,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    4,   -- parent_id
    11,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    43,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    4,   -- parent_id
    15,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    44,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    4,   -- parent_id
    16,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    45,   -- id
    'm:ifInsteadMatcher', -- name
    1,   -- ptype
    '^(.+\. If .+, .+ instead)$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    5,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    46,   -- id
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
    45,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    47,   -- id
    'm:ifInsteadTotal', -- name
    1,   -- ptype
    '^(.+)\. If (.+), (.+) instead$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    46,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    48,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    47,   -- parent_id
    4,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    49,   -- id
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
    47,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    50,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    47,   -- parent_id
    4,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    51,   -- id
    'm:if', -- name
    1,   -- ptype
    '^(If [^\.]+, [^\.]+)$', -- pattern
    'function _Create(text, children, data) return ''TODO'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    5,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    52,   -- id
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
    51,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    53,   -- id
    'm:doSomethingThenAddBoostValue', -- name
    1,   -- ptype
    '^(.+)\. Add its BOOST value to (.+)$', -- pattern
    'function _Create(text, children, data) return ''TODO'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    6,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    54,   -- id
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
    53,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    55,   -- id
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
    53,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    56,   -- id
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
    6,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    57,   -- id
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
    56,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    58,   -- id
    'm:chooseSpaceThenPlaceThere', -- name
    1,   -- ptype
    '^Choose (.+)\. Place (.+) in that space$', -- pattern
    'function _Create(text, children, data) return ''TODO'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    57,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    59,   -- id
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
    58,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    60,   -- id
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
    58,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    61,   -- id
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
    56,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    62,   -- id
    'm:moveMayMoveThroughOpposing', -- name
    1,   -- ptype
    '^[M|m]ove (.+) up to (.+) spaces\. .+ through spaces containing opposing fighters$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:MoveFighters(\n%s,\nUM.Number:UpTo(%s),\ntrue\n)'', children[1], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    61,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    63,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    62,   -- parent_id
    13,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    64,   -- id
    'm:moveMayMoveThroughOpposingAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    62,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    65,   -- id
    'm:draw', -- name
    1,   -- ptype
    '^(.+? )?(may )?[D|d]raws? (.+) cards?$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:Draw(\n%s, \n%s, \n%s\n)'', children[1], children[3], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    8,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    66,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    65,   -- parent_id
    9,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    67,   -- id
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
    65,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    68,   -- id
    'm:drawIsOptionalTrue', -- name
    1,   -- ptype
    '^may $', -- pattern
    'function _Create(text, children, data) return ''true'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    67,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    69,   -- id
    'm:drawIsOptionalFalse', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''false'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    67,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    70,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    65,   -- parent_id
    10,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    71,   -- id
    'm:discard', -- name
    1,   -- ptype
    '^(.+? )?[D|d]iscards? (.+?)( random)? cards?$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:Discard(\n%s, \n%s, \n%s\n)'', children[1], children[2], children[3]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    8,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    72,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    71,   -- parent_id
    9,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    73,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    71,   -- parent_id
    10,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    74,   -- id
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
    71,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    75,   -- id
    'm:discardIsRandomFalse', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''false'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    74,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    76,   -- id
    'm:discardIsRandomTrue', -- name
    1,   -- ptype
    '^ random$', -- pattern
    'function _Create(text, children, data) return ''true'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    74,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    77,   -- id
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
    9,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    78,   -- id
    'm:multiplePlayersEffectOwner', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return '':You()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    77,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    79,   -- id
    'm:multiplePlayersYou', -- name
    1,   -- ptype
    '^[Y|y]ou $', -- pattern
    'function _Create(text, children, data) return '':You()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    77,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    80,   -- id
    'm:multiplePlayersYourOpponent', -- name
    1,   -- ptype
    '^[Y|y]our opponent $', -- pattern
    'function _Create(text, children, data) return '':YourOpponent()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    77,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    81,   -- id
    'm:multiplePlayersEach', -- name
    1,   -- ptype
    '^[E|e]ach (.+)$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    77,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    82,   -- id
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
    81,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    83,   -- id
    'm:multiplePlayersEach', -- name
    1,   -- ptype
    '^opponent $', -- pattern
    'function _Create(text, children, data) return '':Opponents()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    82,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    84,   -- id
    'm:numericStatic', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Number:Static(%s)'', text) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    10,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    85,   -- id
    'm:numericStaticA', -- name
    1,   -- ptype
    '^a$', -- pattern
    'function _Create(text, children, data) return ''UM.Number:Static(1)'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    10,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    86,   -- id
    'm:numericUpTo', -- name
    1,   -- ptype
    '^up to ([0-9])$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Number:UpTo(%s)'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    10,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    87,   -- id
    'm:numericUpToNumber', -- name
    1,   -- ptype
    '^.+$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    86,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    88,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    11,   -- parent_id
    12,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    89,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    11,   -- parent_id
    17,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    90,   -- id
    'm:move', -- name
    1,   -- ptype
    '^[M|m]oves? (.+) up to (.+) spaces$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:MoveFighters(\n%s,\nUM.Number:UpTo(%s),\nfalse\n)'', children[1], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    12,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    91,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    90,   -- parent_id
    13,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    92,   -- id
    'm:moveAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    90,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    93,   -- id
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
    13,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    94,   -- id
    'm:multipleFightersYourFighter', -- name
    1,   -- ptype
    '^[Y|y]our fighter$', -- pattern
    'function _Create(text, children, data) return '':YourFighter()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    95,   -- id
    'm:multipleFightersOpposingFighter', -- name
    1,   -- ptype
    '^[T|t]he opposing fighter$', -- pattern
    'function _Create(text, children, data) return '':Opposing()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    96,   -- id
    'm:multipleFightersEach', -- name
    1,   -- ptype
    '^[E|e]ach (.+)$', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    97,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    96,   -- parent_id
    18,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    98,   -- id
    'm:multipleFightersYourOtherFighter', -- name
    1,   -- ptype
    '^[Y|y]our other fighter$', -- pattern
    'function _Create(text, children, data) return '':OtherThanSource()\n:Your()\n:Single()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    99,   -- id
    'm:multipleFightersAnyFighter', -- name
    1,   -- ptype
    '^[A|a]ny fighter$', -- pattern
    'function _Create(text, children, data) return ''--'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    100,   -- id
    'm:multipleFightersAllOpposingFighters', -- name
    1,   -- ptype
    '^[A|a]ll opposing fighters$', -- pattern
    'function _Create(text, children, data) return '':Opposing()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    101,   -- id
    'm:anySingleFighter', -- name
    1,   -- ptype
    '^(?:any 1|1|an|a) (.+)$', -- pattern
    'function _Create(text, children, data) return children[1]..''\n:Single()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    102,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    101,   -- parent_id
    18,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    103,   -- id
    'm:namedFighters', -- name
    1,   -- ptype
    '^(.+)$', -- pattern
    'function _Create(text, children, data) return string.format('':Named(\''%s\''):Single()'', children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    93,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    104,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    103,   -- parent_id
    14,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    105,   -- id
    'm:cancelAllEffectsOnOpponentsCard', -- name
    1,   -- ptype
    '^Cancel all effects on your opponent''s card$', -- pattern
    'function _Create(text, children, data) return ''UM.Effects:CancelAllEffectsOnOpponentsCard()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    15,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    106,   -- id
    'm:changeSize', -- name
    1,   -- ptype
    '^[C|c]hange size$', -- pattern
    'function _Create(text, children, data) return ''UM.Effects.CharacterSpecific:ChangeSize()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    16,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    107,   -- id
    'm:acquireNewMission', -- name
    1,   -- ptype
    '^Acquire a new mission$', -- pattern
    'function _Create(text, children, data) return ''UM.Effects.CharacterSpecific:AcquireNewMission()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    16,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    108,   -- id
    'm:recover', -- name
    1,   -- ptype
    '^(.+)[R|r]ecovers? (.+) health$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:Recover(\n%s,\n%s\n)'', children[1], children[2]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    17,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    109,   -- id
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
    108,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    110,   -- id
    'm:recoverTargetFighter', -- name
    1,   -- ptype
    '^$', -- pattern
    'function _Create(text, children, data) return ''UM.Select:Fighters():YourFighter():Build()'' end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    109,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    111,   -- id
    'm:recoverTargetMultipleFighters', -- name
    1,   -- ptype
    '^(.+) $', -- pattern
    'function _Create(text, children, data) return children[1] end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    109,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    112,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    111,   -- parent_id
    13,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    113,   -- id
    'm:recoverAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    108,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    114,   -- id
    'm:dealDamage', -- name
    1,   -- ptype
    '^[D|d]eal (.+) damage to (.+)$', -- pattern
    'function _Create(text, children, data) return string.format(''UM.Effects:DealDamage(\n%s,\nUM.Number:Static(%s)\n)'', children[2], children[1]) end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    17,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    115,   -- id
    'm:dealDamageAmount', -- name
    1,   -- ptype
    '^[0-9]$', -- pattern
    'function _Create(text, children, data) return text end', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    114,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    116,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    114,   -- parent_id
    13,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    18,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    18,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    18,   -- parent_id
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
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
    NULL,   -- ref_to_id
    0,
    0
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
    ref_to_id,
    editor_offset_x,
    editor_offset_y
) VALUES (
    127,   -- id
    'REF_NAME', -- name
    1,   -- ptype
    '', -- pattern
    '', -- script
    'test', -- project_name
    '', -- description
    0,   -- is_template
    0,   -- is_root
    123,   -- parent_id
    14,   -- ref_to_id
    0,
    0
);
