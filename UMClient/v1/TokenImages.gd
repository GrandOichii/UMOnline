extends Resource
class_name TokenImages

@export var Images: Array[TokenImage]

var _index = {}

func get_image_for(token_name: String):
	if not token_name in _index:
		for pair in Images:
			if pair.token_name == token_name:
				_index[token_name] = pair.image
				break
	if not token_name in _index:
		push_warning('Image for token ' + token_name + ' not found!')
		return null
	return _index[token_name]