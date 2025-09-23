extends Control
class_name CardImageDisplay

signal Mouseover(card_key: String)

@onready var TextureNode = %Image

var _card_key: String = ''
var _image_loader: CardImageLoader
var _deck_name: String = ''

func load_back():
	TextureNode.texture = _image_loader.get_back_for(_deck_name)

func load(card_key: String) -> void:
	_card_key = card_key
	if _card_key == null:
		load_back()
		return
	TextureNode.texture = _image_loader.get_image_for(card_key)

func _on_mouse_entered() -> void:
	Mouseover.emit(_card_key)

func hide_image():
	%Image.hide()

func show_image():
	%Image.show()

func set_essentials(image_loader: CardImageLoader, deck_name: String):
	_image_loader = image_loader
	_deck_name = deck_name
