import json
from sys import argv
from os import listdir
from os.path import exists
from os import makedirs

LOADOUTS_DIR = argv[1]
SCRIPTS_PATH = argv[2]
FALLBACK_SCRIPTS_PATH = argv[3]
FIGHTER_SCRIPTS = argv[4]
OUT_DIR = argv[5]

TODO_FIGHTER_SCRIPT = '''
function _Create()
    return UM.Build:Fighter()
    -- TODO
    :Build()
end
'''

TODO_CARD_SCRIPT = '''
function _Create()
    return UM.Build:Card()
    -- TODO
    :Build()
end
'''

BASIC_FIGHTER_SCRIPT = '''
function _Create()
    return UM.Build:Fighter()
    :Build()
end
'''

def join(*paths):
    return '/'.join(paths)

# i = 0
for path in listdir(LOADOUTS_DIR):
    loadout_report = {
        'missingScripts': [],
    }
    data = json.loads(open(join(LOADOUTS_DIR, path), 'r').read())
    deck_name = data['Name']
    target_dir = join(OUT_DIR, deck_name)

    # create target directory
    makedirs(target_dir, exist_ok=True)

    # copy/create fighter script
    fighter_scripts_dir = join(target_dir, 'fighters')
    makedirs(fighter_scripts_dir, exist_ok=True)

    for fighter in data['Fighters']:
        fighter_script_path = join(FIGHTER_SCRIPTS, f'{fighter['Name']}.lua')
        fighter_script = TODO_FIGHTER_SCRIPT
        if exists(fighter_script_path):
            fighter_script = open(fighter_script_path, 'r').read()
        
        target_fighter_script_path = join(fighter_scripts_dir, f'{fighter['Name']}.lua')
        open(target_fighter_script_path, 'w').write(fighter_script)
        fighter['Script'] = target_fighter_script_path

    # cards
    card_scripts_path = join(target_dir, 'cards')
    makedirs(card_scripts_path, exist_ok=True)

    for card in data['Deck']:
        card_key = card['Card']['Key'].replace('"', '').replace('?', '')
        card_name = card['Card']['Name']
        card_script = TODO_CARD_SCRIPT

        card_path = join(SCRIPTS_PATH, f'{card_name}.lua')
        has_script = False
        if exists(card_path):
            card_script = open(card_path, 'r').read()
            has_script = True
        else:
            fallback_card_path = join(FALLBACK_SCRIPTS_PATH, f'{card_name}.lua')
            if exists(fallback_card_path):
                card_script = open(fallback_card_path, 'r').read()
                has_script = True
        if not has_script:
            loadout_report['missingScripts'] += [card]
        
        target_card_path = join(card_scripts_path, f'{card_key}.lua')
        open(target_card_path, 'w').write(card_script)
        card['Card']['Script'] = target_card_path

    # copy .json file to target
    open(join(target_dir, f'{deck_name}.json'), 'w').write(json.dumps(data, indent=4))
    open(join(target_dir, 'report.json'), 'w').write(json.dumps(loadout_report, indent=4))
    # i += 1
    # if i == 2:
    #     break