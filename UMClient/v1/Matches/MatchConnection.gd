extends Node
class_name MatchConnection

var cur_data

signal match_info_updated(data)

func _ready():
	match_info_updated.connect(on_match_info_updated)

func respond(_resp: String):
	pass

func on_match_info_updated(data):
	cur_data = data
	# if cur_data.Request != 'Setup': return
	# print(len(cur_data.Setup.Players))

func can_pick_card_in_hand(id):
	if cur_data.Request != 'ChooseCardInHand' and cur_data.Request != 'ChooseCardInHandOrNothing':
		return false
	for key in cur_data.Args:
		if cur_data.Args[key] == id:
			return true
	return false

func pick_card_in_hand(id):
	respond(str(id))

# TODO ChooseNode
# TODO ChooseFighter
# TODO ChooseAttack
