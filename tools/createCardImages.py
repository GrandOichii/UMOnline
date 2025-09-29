from sys import argv
from os import listdir
from os.path import exists, splitext

TARGET = argv[1]

RESULT = '''
[gd_resource type="Resource" script_class="CardImages" load_steps=7 format=3]

[ext_resource type="Script" path="res://v1/CardImage.gd" id="1_bskbv"]
[ext_resource type="Script" path="res://v1/CardImages.gd" id="2_a873h"]
{IMAGE_IMPORTS}

{SUB_RESOURCES}

[resource]
script = ExtResource("2_a873h")
Images = Array[ExtResource("1_bskbv")]([{MINI_SUB_RESOURCES}])
'''

IMAGE_IMPORT = '''
[ext_resource type="Texture2D" path="res://assets/decks/{PATH}" id="{CUSTOM_ID}"]
'''

SUB_RESOURCE = '''
[sub_resource type="Resource" id="Resource_{RID}"]
script = ExtResource("1_bskbv")
card_key = "{NAME}"
image = ExtResource("{CUSTOM_ID}")
'''

MINI_SUB_RESOURCE = '''
SubResource("Resource_{RID}")
'''

MAIN_DIR = 'decks'
DIR = f'UMClient/assets/{MAIN_DIR}'

id = 0

image_imports = ''
subresources = ''
mini_sb = []
for fpath in listdir(DIR):
    for path in listdir(f'{DIR}/{fpath}/cards'):
        if splitext(path)[1] == '.import':
            continue
        back_path = f'{fpath}/cards/{path}'

        # ext_resource

        image_imports += IMAGE_IMPORT.format_map({
            'PATH': back_path,
            'CUSTOM_ID': id,
        })

        # sub_resource
        subresources += SUB_RESOURCE.format_map({
            'RID': id,
            'CUSTOM_ID': id,
            'NAME': splitext(path)[0],
        })

        # mini sub_resource
        mini_sb += [MINI_SUB_RESOURCE.format_map({'RID': id})]

        id += 1

result = RESULT.format_map({
    'IMAGE_IMPORTS': image_imports,
    'SUB_RESOURCES': subresources,
    'MINI_SUB_RESOURCES': ','.join(mini_sb)
})

open(TARGET, 'w', encoding='utf-8').write(result)