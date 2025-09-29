extends PanelContainer
class_name PlayerDisplay

@export var inverted: bool = false
@export var hand_card_scene: PackedScene
@export var FighterScene: PackedScene
@export var image_loader: CardImageLoader
@export var FighterImageLoaderNode: FighterImageLoader
@export var ZoomedCardImage: ZoomedCard
@export var ColorManagerNode: ColorManager

#@onready var MainContainer = %Main
@onready var DeckNode = %Deck
@onready var FightersNode = %Fighters

var _connection

var _deck_name = 'Medusa' # TODO remove

# Called when the node enters the scene tree for the first time
func _ready() -> void:
	DeckNode.set_essentials(image_loader, _deck_name, null)
	%Discard.set_essentials(image_loader, _deck_name, null)
	
	while FightersNode.get_child_count() > 0:
		FightersNode.remove_child(FightersNode.get_child(0))
	while %Hand.get_child_count() > 0:
		%Hand.remove_child(%Hand.get_child(0))

	#if inverted:
		#var count = MainContainer.get_child_count()
		#while count > 0:
			#MainContainer.move_child(MainContainer.get_child(0), count - 1)
			#count -= 1

func set_essentials(connection: MatchConnection):
	_connection = connection

func load_setup(player_setup):
	%Name.text = player_setup.Name
	%Name.add_theme_color_override("font_color", ColorManagerNode.get_player_color(player_setup.Idx))
	_deck_name = player_setup.DeckName
	DeckNode.set_essentials(image_loader, _deck_name, null)
	%Discard.set_essentials(image_loader, _deck_name, null)

	# fighters
	var count = len(player_setup.Fighters)
	while FightersNode.get_child_count() > count:
		FightersNode.remove_child(FightersNode.get_child(0))
	while FightersNode.get_child_count() < count:
		var child = FighterScene.instantiate()
		FightersNode.add_child(child)
		
		var display = child as FighterDisplay
		# TODO
		display.set_essentials(FighterImageLoaderNode, ColorManagerNode)


func load_player(match_data, idx):
	var data = match_data.Players[idx]
	# top data
	%Idx.text = '[%s]' % [data.Idx]
	%Actions.text = str(data.Actions)

	# decks
	# discard
	%DiscardCount.text = str(data.DiscardPile.Count)
	%Discard.hide_image()
	if data.DiscardPile.Count > 0:
		%Discard.load_card(data.DiscardPile.Cards[data.DiscardPile.Count - 1].Key)
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
		
		var display = child as HandCardDisplay
		display.set_essentials(_connection, image_loader, _deck_name, ZoomedCardImage.load_card)
		display.set_idx(%Hand.get_child_count()-1)

	var hi = 0
	for display: HandCardDisplay in %Hand.get_children():
		display.load_card(data.Hand.Cards[hi])
		display.set_idx(hi)
		hi += 1

	# fighters
	var fi = 0
	for fighter in data.Fighters:
		if FightersNode.get_child_count() <= fi: break # TODO fix
		var node = FightersNode.get_child(fi)
		node.load_fighter(fighter)
		fi += 1

	
