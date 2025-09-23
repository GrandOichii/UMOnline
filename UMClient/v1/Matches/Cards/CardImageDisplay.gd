extends Control

signal Mouseover(card_key: String)

@onready var TextureNode = %Image

var _card_key: String = ''

func _ready() -> void:
	pass # Replace with function body.

func load(card_key: String) -> void:
	_card_key = card_key
	# TODO

func _on_mouse_entered() -> void:
	Mouseover.emit(_card_key)
