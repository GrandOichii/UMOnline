extends Node
class_name FighterImageLoader

@export var images: FighterImages

var _index = {}
var _custom_fighter_images = {}

func get_image_for(fighter_id: int, fighter_key: String):
	var key = '%d_%s' % [fighter_id, fighter_key]
	if key in _index:
		return _index[key]
	var result = _get_image(fighter_key)
	_index[key] = result
	return result

func _get_image(fighter_key: String):
	var result = images.get_image_for(fighter_key)
	if result != null:
		return result
	result = _custom_fighter_images.get(fighter_key)
	if result != null:
		return result
	return null

func remember_fighter_images(image_map: Dictionary[String, Texture2D]) -> void:
	for fighter_key in image_map:
		var texture = image_map[fighter_key]
		_custom_fighter_images.set(fighter_key, texture)