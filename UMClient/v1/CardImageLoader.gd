class_name CardImageLoader extends Node

@export var images: CardImages
@export var backs: CardImages
@export var card_default: Texture2D
@export var card_back_default: Texture2D

func get_image_for(card_key: String):
	var result = images.get_image_for(card_key)
	if result == null:
		return card_default
	return result
	
func get_back_for(deck_name: String):
	var result = backs.get_image_for(deck_name)
	if result == null:
		return card_back_default
	return result
