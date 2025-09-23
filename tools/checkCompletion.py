import json
from os.path import join, exists

DECKS_PATH = 'decks.json'
IMPLEMENTATIONS_DIR = 'card-scripts'

data = json.loads(open(DECKS_PATH, 'r').read())

result = []

for deck in data['decks']:
    implemented = 0
    for card in deck['cards']:
        name = card['title'].replace('?', ' ').replace('!', ' ').replace(':', ' ').replace('"', ' ').strip()
        card_path = join(IMPLEMENTATIONS_DIR, f'{name}.lua')
        if exists(card_path):
            implemented += 1
    result += [{
        'name': deck['name'],
        'implemented': implemented,
        'total': len(deck['cards'])
    }]

result = sorted(result, key=lambda deck: deck['total'] - deck['implemented'])

for deck in result:
    print('{}: {}/{}'.format(deck['name'], deck['implemented'], deck['total']))