import json

DECKS_FILE = 'decks.json'
FIGHTER_SCRIPTS_DIR_FORMAT = '../fighter-scripts/{}.lua'
CARD_SCRIPT_DIR_FORMAT = '../{}/{}.lua'
TARGET_DIR_FORMAT = 'loadouts-generated/{}.json'


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
        'The Sisters': 'Sisters',
        'Outlaws': 'Outlaw',
        'Dr. Malcolm': 'Dr. Malcolm',
        'The Jabberwock': 'The Jabberwock',
        'Maria Hill': 'Maria Hill',
        'Dr. Watson': 'Dr. Watson',
        'Giles or Xander': 'GilesXander',
        'Rosencrantz & Guildenstern': 'Rosencrantz & Guildenstern',
        'The Porter': 'The Porter',
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
            'Movement': deck['movement'],
            'Script': FIGHTER_SCRIPTS_DIR_FORMAT.format(fighter['name'])
        }]
    for fighter in deck['sidekicks']:
        fighters += [{
            'Name': get_sidekick_name(fighter['name']),
            'Key': get_sidekick_name(fighter['name']),
            'Amount': fighter['quantity'],
            'IsHero': False,
            'MaxHealth': fighter['hp'],
            'StartingHealth': fighter['hp'],
            'IsRanged': fighter['attack_type'] == 'ranged',
            'Text': deck['special'],
            'Movement': deck['movement'],
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

    open(TARGET_DIR_FORMAT.format(deck_name), 'w').write(json.dumps(loadout, indent=4))