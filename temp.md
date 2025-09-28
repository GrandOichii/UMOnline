1. use decks.json to generate individual loadout json file
2. Parse all cards in generated loadout files
3. for each loadout file:
    1. create a directory for scripts (use deck name)
    2. search for the script in custom implemented cards
    3. if doesnt exist, search for the script in card-scripts
    4. if script exists, copy it into loadout scripts directory
    5. else paste a temporary file that creates a blank card