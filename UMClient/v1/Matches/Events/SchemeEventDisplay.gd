extends EventDisplay
class_name SchemeEventDisplay

@onready var FighterNode = %Fighter
@onready var CardNode = %Card

func _ready() -> void:
	pass # Replace with function body.
	
func set_essentials(card_loader: CardImageLoader, fighter_loader: FighterImageLoader, on_card_mouseover):
	CardNode.set_essentials(card_loader, '', on_card_mouseover)
	FighterNode.set_essentials(fighter_loader)

func load_event(e):
	%Player.text = e.PlayerName
	CardNode.load_card(e.Card.Key)
	FighterNode.load_fighter(e.Fighter.Id, e.Fighter.Name)
