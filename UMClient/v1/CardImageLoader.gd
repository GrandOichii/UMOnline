class_name CardImageLoader extends Node

@export var images: CardImages
@export var backs: CardImages
@export var card_default: Texture2D
@export var card_back_default: Texture2D

var _custom_deck_backs = {}
var _custom_card_images = {}

func get_image_for(card_key: String) -> Texture2D:
	var result = images.get_image_for(card_key)
	if result != null:
		return result
	result = _custom_card_images.get(card_key)
	if result != null:
		return result
	return null
	
func get_back_for(deck_name: String) -> Texture2D:
	var result = backs.get_image_for(deck_name)
	if result != null:
		return result
	result = _custom_deck_backs.get(deck_name)
	if result != null:
		return result
	return null

func remember_card_back(deck_name: String, texture: Texture2D):
	_custom_deck_backs.set(deck_name, texture)

func remember_card_images(image_map: Dictionary[String, Texture2D]) -> void:
	for card_key in image_map:
		var texture = image_map[card_key]
		_custom_card_images.set(card_key, texture)
