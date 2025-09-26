extends Control
class_name CombatPartDisplay

@export var IsDefender: bool
@export var DefenderIcon: Texture2D

@onready var TopContainer: BoxContainer = %Top
@onready var BottomContainer: BoxContainer = %Bottom

var _card_image_loader
var _fighter_image_loader

func _ready() -> void:
	if IsDefender:
		BottomContainer.alignment = BoxContainer.ALIGNMENT_BEGIN
		%PartIcon.texture = DefenderIcon
		var bcount = BottomContainer.get_child_count()
		while bcount > 0:
			BottomContainer.move_child(BottomContainer.get_child(0), bcount - 1)
			bcount -= 1
			
		TopContainer.alignment = BoxContainer.ALIGNMENT_BEGIN
		var tcount = TopContainer.get_child_count()
		while tcount > 0:
			TopContainer.move_child(TopContainer.get_child(0), tcount - 1)
			tcount -= 1

func set_essentials(card_image_loader: CardImageLoader, fighter_image_loader: FighterImageLoader):
	_card_image_loader = card_image_loader
	_fighter_image_loader = fighter_image_loader
	
	%Card.set_essentials(card_image_loader, '', null) # TODO
	%Fighter.set_essentials(fighter_image_loader)

func load_combat(match_data):
	var data = match_data.Combat
	var fighter = data.Attacker
	var card = data.AttackCard
	
	if IsDefender:
		fighter = data.Defender
		card = data.DefenceCard
	_set_fighter(fighter)

	if card == null:
		%Boosts.hide()
		%Card.hide()
		%Power.text = '0'
		return
	%Boosts.hide()
	%Card.show()

	%Card.set_deck_name(card.DeckName)
	_set_card_value(card)
	_set_card(card)
	_set_boosts(card)
	
func _set_fighter(fighter):
	%Fighter.load_fighter(fighter)
	
func _set_card_value(card_data):
	if card_data.Value == null:
		%Power.text = '?'
		return
	%Power.text = str(card_data.Value)

func _set_boosts(card_data):
	if len(card_data.Boosts) == 0:
		%Boosts.hide()
		return
	%Boosts.show()
	# TODO

func _set_card(card_data):
	if card_data.Card == null:
		%Card.load_back()
		return
	%Card.load_card(card_data.Card.Key)
