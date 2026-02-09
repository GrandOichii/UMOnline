import json
from os.path import join, exists

DECKS_PATH = 'decks.json'
IMPLEMENTATIONS_DIR = '.generated/scripts'
CUSTOM_IMPLEMENTATIONS_DIR = 'custom-card-scripts'

data = json.loads(open(DECKS_PATH, 'r').read())

result = []

for deck in data['decks']:
    implemented = 0
    for card in deck['cards']:
        name = card['title'].replace('?', ' ').replace('!', ' ').replace(':', ' ').replace('"', ' ').strip()
        card_path = join(IMPLEMENTATIONS_DIR, f'{name}.lua')
        if exists(card_path):
            text = open(card_path, 'r').read()
            if 'unfinished card' not in text:
                implemented += 1
        card_path = join(CUSTOM_IMPLEMENTATIONS_DIR, f'{name}.lua')
        if exists(card_path):
            implemented += 1
    result += [{
        'name': deck['name'],
        'implemented': implemented,
        'total': len(deck['cards'])
    }]

result = sorted(result, key=lambda deck: deck['total'] - deck['implemented'])

ONLY_DECKS = [
    'King Arthur',
    'Alice',
    'Medusa',
    'Sinbad',
    'Bigfoot',
    'Robin Hood',
]

for deck in result:
    if not deck['name'] in ONLY_DECKS:
        continue
    print('{}: {}/{}'.format(deck['name'], deck['implemented'], deck['total']))