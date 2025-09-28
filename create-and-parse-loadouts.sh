#!/bin/sh

mkdir loadouts-generated
echo 'Creating JSON loadouts...'
python3 tools/createLoadouts.py
echo 'JSON loadouts created'

echo 'Parsing card texts...'
# TODO create cards.json
python3 tools/createCardsJson.py loadouts-generated .cards.json
mkdir .generated-scripts
mkdir .generated-reports
# cd Parser
# dotnet run ../.cards.json .generated-reports .generated-scripts 
# echo 'Card texts parsed'