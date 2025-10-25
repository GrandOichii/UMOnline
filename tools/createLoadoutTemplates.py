import json
from sys import argv
from os.path import join
import re

DECKS_FILE = 'decks.json'
FIGHTER_SCRIPTS_DIR_FORMAT = join('..', 'fighter-scripts', '{}.lua')
CARD_SCRIPT_DIR_FORMAT = join('..', '{}', '{}.lua')
TARGET_DIR_FORMAT = join(argv[1], '{}.json')

data = json.loads(open(DECKS_FILE, 'r').read())

def get_allowed_fighters(af):
    if af == 'ANY':
        return []
    return [{
        'Arthur': 'King Arthur',
        'Medusa': 'Medusa',
        'Harpy': 'Harpy',
        'Daredevil': 'Daredevil',
        'Sinbad': 'Sinbad',
        'Porter': 'Porter',
        'Giles': 'Giles',
        'Xander': 'Xander',
        'Buffy': 'Buffy',
        'Huntsman': 'Huntsman',
        'Willow': 'Willow',
        'Tara': 'Tara',
        'Luke cage': 'Luke Cage',
        'Misty knight': 'Misty Knight',
        'Bloody mary': 'Bloody Mary',
        'Patroclus': 'Patroclus',
        'Black panther': 'Black Panther',
        'Achilles': 'Achilles',
        'Muldoon': 'Muldoon',
        'Ingen worker': 'Ingen Worker',
        'Shuri': 'Shuri',
        'Yennenga': 'Yennenga',
        'Archer': 'Archer',
        'Bullseye': 'Bullseye',
        'Houdini': 'Houdini',
        'Bess': 'Bess',
        'Squirrel': 'Squirrel',
        'Squirrel girl': 'Squirrel Girl',
        'Ghost rider': 'Ghost Rider',
        'Invisible man': 'Invisible Man',
        'Dr. jekyll': 'Dr. Jekyll',
        'Mr. hyde': 'Mr. Hyde',
        'Angel': 'Angel',
        'Ciri': 'Ciri',
        'Ihuarraquax': 'Ihuarraquax',
        'Leshen': 'Leshen',
        'Wolf': 'Wolf',
        'Eredin': 'Eredin',
        'Red rider': 'Red Rider',
        'Titania': 'Titania',
        'Oberon': 'Oberon',
        'Philippa': 'Philippa',
        'Dijkstra': 'Dijkstra',
        'Elektra': 'Elektra',
        'The hand': 'The Hand',
        'Little red': 'Little Red',
        'Beowulf': 'Beowulf',
        'T-rex': 'T-Rex',
        'Robin': 'Robin',
        'Cloak': 'Cloak',
        'Dagger': 'Dagger',
        'The genie': 'The Genie',
        'Winter soldier': 'Winter Soldier',
        'Outlaw': 'Outlaw',
        'Tesla': 'Tesla',
        'Shakespeare': 'Shakespeare',
        'Actor': 'Actor',
        'Jill trent': 'Jill Trent',
        'Dracula': 'Dracula',
        'Daisy': 'Daisy',
        'Annie': 'Annie',
        'Charlie': 'Charlie',
        'Wong': 'Wong',
        'Geralt': 'Geralt',
        'Dandelion': 'Dandelion',
        'Yennefer': 'Yennefer',
        'Triss': 'Triss',
        'Merlin': 'Merlin',
        'Honor guard': 'Honor Guard',
        'Oda nobunaga': 'Oda Nobunaga',
        'Tomoe gozen': 'Tomoe Gozen',
        'Doctor strange': 'Doctor Strange',
        'Spider-man': 'Spider-Man',
        'She-hulk': 'She-Hulk',
        'Golden bat': 'Golden Bat',
        'Sister': 'Sister',
        'Bigfoot': 'Bigfoot',
        'Jackalope': 'Jackalope',
        'Wiglaf': 'Wiglaf',
        'Dr. sattler': 'Dr. Sattler',
        'Dr. malcolm': 'Dr. Malcolm',
        'Faith': 'Faith',
        'Spike': 'Spike',
        'Drusilla': 'Drusilla',
        'Jabberwock': 'Jabberwock',
        'Alice': 'Alice',
        'Hamlet': 'Hamlet',
        'Maria hill': 'Maria Hill',
        'Black widow': 'Black Widow',
        'Dr. watson': 'Dr. Watson',
        'Holmes': 'Sherlock Holmes',
        'Ms. marvel': 'Ms. Marvel',
    }[af.lower().capitalize()]]

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
        'The Sisters': 'Sister',
        'Outlaws': 'Outlaw',
        'Dr. Malcolm': 'Dr. Malcolm',
        'The Jabberwock': 'The Jabberwock',
        'Maria Hill': 'Maria Hill',
        'Dr. Watson': 'Dr. Watson',
        'Giles or Xander': 'GilesXander',
        'Rosencrantz & Guildenstern': 'Rosencrantz & Guildenstern',
        'The Porter': 'The Porter',
    }[plural]

def get_card_name(title):
    m = re.match('^(.+)_.+$', title)
    if m is None:
        return title
    return m.groups()[0]

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
            # 'Script': FIGHTER_SCRIPTS_DIR_FORMAT.format(fighter['name'])
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
            'Text': '',
            'Movement': deck['movement'],
            # 'Script': FIGHTER_SCRIPTS_DIR_FORMAT.format('Basic')
        }]
    for card in deck['cards']:
        cards += [{
            'Amount': card['quantity'],
            'Card': {
                'AllowedFighters': get_allowed_fighters(card['characterName']),
                'Name': get_card_name(card['title']) ,
                'Key': f'{deck_name}_{card['title']}',
                'Type': card['type'].capitalize(),
                'Value': card['value'],
                'Boost': card['boost'],
                'Text': get_text(card),
                'Labels': card['labels'] if 'labels' in card else [],
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