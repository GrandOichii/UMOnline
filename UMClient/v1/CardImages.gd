extends Resource
class_name CardImages

@export var Images: Array[CardImage]

var _index = {}

func get_image_for(card_key: String):
	if not card_key in _index:
		for pair in Images:
			if pair.card_key == card_key:
				_index[card_key] = pair.image
				break
	if not card_key in _index:
		push_warning('Image for card key ' + card_key + ' not found!')
		return null
	return _index[card_key]
