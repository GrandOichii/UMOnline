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
    1,                      -- id
    'parser1',              -- name
    1,                      -- ptype
    'Draw ([0-9]+) cards.', -- pattern
    'function _Create(text, children, data) return ''UM.Effects:Draw(''..children[1]..'')'' end',                     -- script
    'test',                 -- project_name
    'parser1 description',  -- description
    1,                      -- is_template
    1,                      -- is_root
    NULL,                   -- parent_id
    0                       -- is_ref
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
    2,                      -- id
    'parser2',              -- name
    2,                      -- ptype
    '',                     -- pattern
    '',                     -- script
    'test',                 -- project_name
    'parser2 description',  -- description
    0,                      -- is_template
    0,                      -- is_root
    1,                      -- parent_id
    0                       -- is_ref
);