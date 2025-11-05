extends Node
class_name FighterImageLoader

@export var images: FighterImages

var _index = {}

func get_image_for(fighter_id: int, fighter_key: String):
	var key = '%d_%s' % [fighter_id, fighter_key]
	if key in _index:
		return _index[key]
	var result = images.get_image_for(fighter_key)
	_index[key] = result
	return result
