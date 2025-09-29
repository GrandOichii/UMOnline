from sys import argv
from os import listdir
from os.path import exists, splitext
import re


def get_sidekick_name(plural):
    return {
        'Harpies': 'Harpy',
        'Faith': 'Faith',
        'Drusilla': 'Drusilla',
        'Patroclus': 'Patroclus',
        'Oberon': 'Oberon',
        'Tara': 'Tara',
        'Shuri': 'Shuri',
        'Daisy': 'Daisy',
        'Charlie': 'Charlie',
        'Wong': 'Wong',
        'Dandelion': 'Dandelion',
        'Triss': 'Triss',
        'April O\'Neil': 'April O\'Neil',
        'Metalhead': 'Metalhead',
        'Merlin': 'Merlin',
        'Bebop & Rocksteady': 'Bebop & Rocksteady',
        'Honor Guard': 'Honor Guard',
        'Actors': 'Actor',
        'The Hand': 'The Hand',
        'Casey Jones': 'Casey Jones',
        'Splinter': 'Splinter',
        'Dijkstra': 'Dijkstra',
        'Bess': 'Bess',
        'Red Riders': 'Red Rider',
        'Wolves': 'Wolf',
        'Ihuarraquax': 'Ihuarraquax',
        'Archers': 'Archer',
        'Ingen Workers': 'Ingen Worker',
        'Clones': 'Clone',
        'Misty Knight': 'Misty Knight',
        'Huntsman': 'Huntsman',
        'The Jackalope': 'The Jackalope',
        'Wiglaf': 'Wiglaf',
        'Wolf': 'Wolf',
        'Seadog': 'Seadog',
        'Giles': 'Giles',
        'Red Rider': 'Red Rider',
        'Ingen Worker': 'Ingen Worker',
        'Kakodaemon': 'Kakodaemon',
        'Squirrel': 'Squirrel',
        'Actor': 'Actor',
        'The Sisters': 'Sister',
        'Outlaws': 'Outlaw',
        'Dr. Malcolm': 'Dr. Malcolm',
        'The Jabberwock': 'The Jabberwock',
        'Maria Hill': 'Maria Hill',
        'Dr. Watson': 'Dr. Watson',
        'Xander': 'Xander',
        'Yennefer': 'Yennefer',
        'Giles or Xander': 'GilesXander',
        'Rosencrantz & Guildenstern': 'Rosencrantz & Guildenstern',
        'The Porter': 'The Porter',
    }[plural]

TARGET = argv[1]

MAIN_DIR = 'decks'
DIR = f'UMClient/assets/{MAIN_DIR}'

RESULT = '''
[gd_resource type="Resource" script_class="FighterImages" format=3]

[ext_resource type="Script" path="res://v1/FighterImage.gd" id="1_gr0k2"]
[ext_resource type="Script" path="res://v1/FighterImages.gd" id="2_25rj5"]
{EXT_RESOURCES}

{SUB_RESOURCES}

[resource]
script = ExtResource("2_25rj5")
Images = Array[ExtResource("1_gr0k2")]([{MINI_SUB_RESOURCES}])
'''

EXT_RESOURCE = '''
[ext_resource type="Texture2D" path="res://assets/decks/{PATH}" id="{IMAGE_ID}"]
'''

MINI_EXT_RESOURCE = '''
ExtResource("{IMAGE_ID}")
'''

SUB_RESOURCE = '''
[sub_resource type="Resource" id="Resource_{FIGHTER_ID}"]
script = ExtResource("1_gr0k2")
fighter_key = "{NAME}"
images = Array[CompressedTexture2D]([{MINI_EXT_RESOURCES}])
'''

MINI_SUB_RESOURCE = '''
SubResource("Resource_{FIGHTER_ID}")
'''

fighter_id = 0
image_id = 0
ext_resources = ''
sub_resources = ''
mini_sb = []

for fpath in listdir(DIR):
    # hero
    hero_path = f'{fpath}/heroes/hero.png'
    if not exists(f'{DIR}/{hero_path}'):
        hero_path = f'{fpath}/heroes/hero.jpg'
    if not exists(f'{DIR}/{hero_path}'):
        hero_path = f'{fpath}/heroes/hero.webp'

    # ext_resource
    ext_resources += EXT_RESOURCE.format_map({
        'PATH': hero_path,
        'IMAGE_ID': image_id
    })

    # sub_resource
    sub_resources += SUB_RESOURCE.format_map({
        'FIGHTER_ID': fighter_id,
        'NAME': fpath,
        'MINI_EXT_RESOURCES': MINI_EXT_RESOURCE.format_map({'IMAGE_ID': image_id})
    })

    # mini sub_resource
    mini_sb += [MINI_SUB_RESOURCE.format_map({'FIGHTER_ID': fighter_id})]

    fighter_id += 1
    image_id += 1

    # sidekicks
    name = ''
    sidekicks = {}
    for path in listdir(f'{DIR}/{fpath}/sidekicks'):
        if splitext(path)[1] == '.import':
            continue
        sidekick_path = f'{fpath}/sidekicks/{path}'
        name = get_sidekick_name(re.match(r'^.+_(.+)_[0-9].+$', sidekick_path).groups()[0])
        if not name in sidekicks:
            sidekicks[name] = []
        sidekicks[name] += [sidekick_path]

    for sidekick in sidekicks:
        images = []
        for image_path in sidekicks[sidekick]:
            # ext_resource
            ext_resources += EXT_RESOURCE.format_map({
                'PATH': image_path,
                'IMAGE_ID': image_id
            })
            images += [MINI_EXT_RESOURCE.format_map({'IMAGE_ID': image_id})]
            image_id += 1
        # sub_resource
        sub_resources += SUB_RESOURCE.format_map({
            'FIGHTER_ID': fighter_id,
            'NAME': sidekick,
            'MINI_EXT_RESOURCES': ','.join(images)
        })

        # mini sub_resource
        mini_sb += [MINI_SUB_RESOURCE.format_map({'FIGHTER_ID': fighter_id})]
        fighter_id += 1

result = RESULT.format_map({
    'EXT_RESOURCES': ext_resources,
    'SUB_RESOURCES': sub_resources,
    'MINI_SUB_RESOURCES': ','.join(mini_sb)
})

open(TARGET, 'w', encoding='utf-8').write(result)