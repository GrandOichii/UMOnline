extends Node
class_name ColorManager

@export var Decks: DeckColors

var _player_map = {}
var _fighter_map = {}

func load_setup(setup_data):
	for p in setup_data.Players:
		var color = get_deck_color(p.DeckName)
		_player_map[int(p.Idx)] = color
		for f in p.Fighters:
			_fighter_map[int(f.Id)] = color

func get_player_color(player_idx):
	return _player_map[int(player_idx)]

func get_fighter_color(fighter_id):
	return _fighter_map[int(fighter_id)]

func get_deck_color(deck_name: String):
	for d in Decks.Decks:
		if deck_name == d.DeckName:
			return d.DeckColor
#	TODO throw error
