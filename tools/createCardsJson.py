import json
from sys import argv
from os import listdir
from os.path import join

LOADOUTS_DIR = argv[1]
OUT_PATH = argv[2]

result = []
parsed = []

for path in listdir(LOADOUTS_DIR):
    data = json.loads(open(join(LOADOUTS_DIR, path), 'r').read())
    for card in data['Deck']:
        name = card['Name']
        if name in parsed: 
            continue
        parsed += [name]
        result += [{
            'name': name,
            'type': card['Type'],
            'power': card['Value'],
            'text': card['Text'],
            'labels': card['Labels'],
        }]

open(OUT_PATH, 'w').write(json.dumps(result, indent=4))