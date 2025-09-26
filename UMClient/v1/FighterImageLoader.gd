extends Node
class_name FighterImageLoader

@export var images: FighterImages

var _index = {}

func get_image_for(fighter_id: int, fighter_key: String):
	if fighter_id in _index:
		return _index[fighter_id]
	var result = images.get_image_for(fighter_key)
	_index[fighter_id] = result
	return result
