extends Control

@export var Connection: MatchConnection

@onready var Display = %Display

@onready var HintLabel = %HintLabel
@onready var RequestLabel = %RequestLabel
@onready var ResponseEdit = %ResponseEdit
@onready var OptionsDisplay = %OptionsDisplay

func _ready() -> void:
	Connection.match_info_updated.connect(_on_match_info_updated)
	Display.set_connection(Connection)

func remember_deck_card_back(deck_name: String, texture: Texture2D) -> void:
	if texture == null:
		return
	Display.CardImageLoaderNode.remember_card_back(deck_name, texture)

func remember_deck_card_images(image_map: Dictionary[String, Texture2D]) -> void:
	Display.CardImageLoaderNode.remember_card_images(image_map)
	
func remember_deck_fighter_images(image_map: Dictionary[String, Texture2D]) -> void:
	Display.FighterImageLoaderNode.remember_fighter_images(image_map)
	
func _on_match_info_updated(data):
	if data.Request == 'Setup':
		Display.load_setup(data.Setup)
	Display.load_connected_match(data)

	HintLabel.text = data.Hint
	RequestLabel.text = data.Request
	OptionsDisplay.text = str(data.Args)

func _on_send_button_pressed() -> void:
	Connection.respond(ResponseEdit.text)
	ResponseEdit.clear()
