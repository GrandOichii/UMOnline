from sys import argv
import sqlite3
import json

DB_PATH = argv[1]
CARDS_JSON_PATH = argv[2]

connection = sqlite3.connect(DB_PATH)
cursor = connection.cursor()

cards = json.loads(open(CARDS_JSON_PATH, 'r').read())

for card in cards:
    cursor.execute('''
    UPDATE cards
    SET text = ?
    WHERE name = ?
    ''', (card['text'], card['name']))
    connection.commit()
    print(f'Updated text for card {card['name']}')


cursor.close()
connection.close()