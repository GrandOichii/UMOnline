#!/bin/sh

rm -rf .generated
mkdir .generated

mkdir .generated/loadout-templates
echo 'Creating JSON loadouts...'
python3 tools/createLoadoutTemplates.py\
    .generated/loadout-templates
echo 'JSON loadouts created'

python3 tools/createCardsJson.py\
    .generated/loadout-templates\
    .generated/cards.json
mkdir .generated/scripts
mkdir .generated/reports
echo 'Created cards.json'

echo 'Parsing card texts...'
python3 tools/mapScripts.py\
    mappedtexts.json\
    .generated/cards.json\
    .generated/scripts
echo 'Card texts parsed'

mkdir .generated/loadouts

python3 tools/bakeLoadouts.py\
    .generated/loadout-templates\
    .generated/scripts\
    custom-card-scripts\
    fighter-scripts\
    .generated/loadouts
