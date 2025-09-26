extends Resource
class_name FighterImages

@export var Images: Array[FighterImage]

var _index = {}
var _fighter_idx = {}

func get_image_for(fighter_key: String):
	if not fighter_key in _index:
		for pair in Images:
			if pair.fighter_key == fighter_key:
				_fighter_idx[fighter_key] = 0
				_index[fighter_key] = pair.images
				break
	if not fighter_key in _index:
		push_warning('Image for fighter key ' + fighter_key + ' not found!')
		return null
	
	var idx = _fighter_idx[fighter_key]
	_fighter_idx[fighter_key] = (idx + 1) % len(_index[fighter_key])
	return _index[fighter_key][idx]
