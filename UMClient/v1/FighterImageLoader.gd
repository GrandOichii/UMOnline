extends Node
class_name FighterImageLoader

@export var images: FighterImages

func get_image_for(card_key: String):
	return images.get_image_for(card_key)
	
