extends PanelContainer
class_name ZoomedCard

@export var ImageLoader: CardImageLoader

@onready var Card: CardImageDisplay = %Card

func _ready() -> void:
	Card.set_essentials(ImageLoader, '', null)

func load_card(card_key: String):
	if card_key == '':
		return
	%Card.load_card(card_key)
