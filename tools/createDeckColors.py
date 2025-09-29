# '#%02x%02x%02x' % (0, 128, 64)
from PIL import ImageColor
import json

from sys import argv

RESULT = '''
[gd_resource type="Resource" script_class="DeckColors" load_steps=6 format=3 uid="uid://0r366endf36k"]

[ext_resource type="Script" path="res://v1/Decks/DeckColor.gd" id="1_388kc"]
[ext_resource type="Script" path="res://v1/Decks/DeckColors.gd" id="2_3o2nn"]

{SUB_RESOURCES}

[resource]
script = ExtResource("2_3o2nn")
Decks = Array[ExtResource("1_388kc")]([{MINI_SUB_RESOURCES}])
'''

SUB_RESOURCE = '''
[sub_resource type="Resource" id="Resource_{ID}"]
script = ExtResource("1_388kc")
DeckName = "{NAME}"
DeckColor = Color({R}, {G}, {B}, 1)
'''

MINI_SUB_RESOURCE = '''
SubResource("Resource_{ID}")
'''

IN = 'tools/colors.json'

OUT = argv[1]

data = json.loads(open(IN, 'r').read())
sub_r = ''
id = 0
mini_sub = []
for name in data:
    r, g, b = ImageColor.getcolor(f'#{data[name]}', 'RGB')
    sub_r += SUB_RESOURCE.format_map({
        'NAME': name,
        'ID': id,
        'R': r/256,
        'G': g/256,
        'B': b/256,
    })
    mini_sub += [MINI_SUB_RESOURCE.format_map({'ID': id})]
    id += 1


open(OUT, 'w').write(RESULT.format_map({
    'SUB_RESOURCES': sub_r,
    'MINI_SUB_RESOURCES': ','.join(mini_sub)
}))