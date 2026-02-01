import json
from sys import argv
from os.path import join

MAPPED_TEXTS_PATH = argv[1]
CARDS_PATH = argv[2]
OUT_DIR = argv[3]

def name_to_file_path(name):
    n = name.replace('"', '').replace('?', '')
    return join(OUT_DIR, f'{n}.lua')

# create text map
mapped_texts = json.loads(open(MAPPED_TEXTS_PATH, 'r').read())
script_map = {}
for text in mapped_texts:
    if 'TODO' in text['script']:
        continue
    script_map[text['text']] = text['script']

# read cards
cards = json.loads(open(CARDS_PATH, 'r').read())

# map scripts and save to files
for card in cards:
    script = 'DEFAULT SCRIPT' # TODO
    if card['text'] in script_map:
        script = script_map[card['text']]
        # print('YES')
    # print(script)
    open(name_to_file_path(card['name']), 'w').write(script)