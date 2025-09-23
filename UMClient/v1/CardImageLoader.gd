class_name CardImageLoader extends Node

@export var images: CardImages
@export var backs: CardImages

func get_image_for(card_key: String):
	return images.get_image_for(card_key)
	
func get_back_for(deck_name: String):
	return backs.get_image_for(deck_name)
