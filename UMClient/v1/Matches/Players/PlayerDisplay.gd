extends PanelContainer

@export var inverted: bool = false
@export var hand_card_scene: PackedScene
@export var image_loader: CardImageLoader
@export var ZoomedCardImage: ZoomedCard

@onready var MainContainer = %Main
@onready var DeckNode = %Deck

var _deck_name = 'medusa & harpies' # TODO remove

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	DeckNode.set_essentials(image_loader, _deck_name, null)
	%Discard.set_essentials(image_loader, _deck_name, null) # TODO

	if inverted:
		var count = MainContainer.get_child_count()
		while count > 0:
			MainContainer.move_child(MainContainer.get_child(0), count - 1)
			count -= 1

func load_player(match_data, idx):
	var data = match_data.Players[idx]
	# top data
	%Idx.text = '[%s]' % [data.Idx]
	%Name.text = data.Name
	%Actions.text = str(data.Actions)

	# decks
	# discard
	%DiscardCount.text = str(data.DiscardPile.Count)
	%Discard.hide_image()
	if data.DiscardPile.Count > 0:
		%Discard.show_image()
	# deck
	%DeckCount.text = str(data.Deck.Count)
	DeckNode.load_back()
	DeckNode.hide_image()
	if data.Deck.Count > 0:
		DeckNode.show_image()
	
	# hand
	var count = len(data.Hand.Cards)
	while %Hand.get_child_count() > count:
		%Hand.remove_child(%Hand.get_child(0))
	while %Hand.get_child_count() < count:
		var child = hand_card_scene.instantiate()
		%Hand.add_child(child)
		
		var display = child as CardImageDisplay
		display.set_essentials(image_loader, _deck_name, ZoomedCardImage.load_card)

	var i = 0
	for display: CardImageDisplay in %Hand.get_children():
		display.load_hand_card(data.Hand.Cards[i])
		i += 1
	
