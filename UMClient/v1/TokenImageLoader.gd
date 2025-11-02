class_name TokenImageLoader
extends Node

@export var images: TokenImages

func get_image_for(token_name: String):
	return images.get_image_for(token_name)
