import json

DECKS_FILE = 'decks.json'
FIGHTER_SCRIPTS_DIR_FORMAT = '../fighter-scripts/{}.lua'
CARD_SCRIPT_DIR_FORMAT = '../{}/{}.lua'

data = json.loads(open(DECKS_FILE, 'r').read())

def get_allowed_fighters(af):
    if af == 'ANY':
        return []
    return [af.lower().capitalize()]

def get_text(card):
    _m = {
        'basicText': '{}',
        'immediateText': 'Immediately: {}',
        'duringText': 'During combat: {}',
        'afterText': 'After combat: {}',
    }
    lines = []
    for key in _m:
        if not key in card or card[key] == None:
            continue
        v = card[key]
        lines += [_m[key].format(v)]

    return '\n'.join(lines)

def get_sidekick_name(plural):
    return {
        'Harpies': 'Harpy'
    }[plural]

for deck in data['decks']:
    deck_name = deck['name']
    deck_key = deck['name']

    fighters = []
    cards = []

    for fighter in deck['heroes']:
        fighters += [{
            'Name': fighter['name'],
            'Key': fighter['name'],
            'Amount': fighter['quantity'],
            'IsHero': True,
            'MaxHealth': fighter['hp'],
            'StartingHealth': fighter['hp'],
            'IsRanged': fighter['attack_type'] == 'ranged',
            'Text': deck['special'],
            'Script': FIGHTER_SCRIPTS_DIR_FORMAT.format(fighter['name'])
        }]
    for fighter in deck['sidekicks']:
        fighters += [{
            'Name': get_sidekick_name(fighter['name']),
            'Key': get_sidekick_name(fighter['name']),
            'Amount': fighter['quantity'],
            'IsHero': True,
            'MaxHealth': fighter['hp'],
            'StartingHealth': fighter['hp'],
            'IsRanged': fighter['attack_type'] == 'ranged',
            'Text': deck['special'],
            'Script': FIGHTER_SCRIPTS_DIR_FORMAT.format('Basic')
        }]
    for card in deck['cards']:
        cards += [{
            'Amount': card['quantity'],
            'Card': {
                'AllowedFighters': get_allowed_fighters(card['characterName']),
                'Name': card['title'],
                'Key': f'{deck_name}_{card['title']}',
                'Type': card['type'].capitalize(),
                'Value': card['value'],
                'Boost': card['boost'],
                'Text': get_text(card),
                'Script': CARD_SCRIPT_DIR_FORMAT.format(deck_name, card['title'])
            }
        }]

    loadout = {
        'Name': deck_name,
        'Key': deck_key,
        'StartsWithSidekicks': True,
        'Fighters': fighters,
        'Deck': cards
    }
    print(json.dumps(loadout, indent=4))
    break