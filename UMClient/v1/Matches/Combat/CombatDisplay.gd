extends HBoxContainer
class_name CombatDisplay

@export var CardImageLoaderNode: CardImageLoader
@export var FighterImageLoaderNode: FighterImageLoader
@export var ColorManagerNode: ColorManager

func _ready() -> void:
	set_essentials(CardImageLoaderNode, FighterImageLoaderNode)
		
func set_essentials(card_loader: CardImageLoader, fighter_loader: FighterImageLoader):
	CardImageLoaderNode = card_loader
	FighterImageLoaderNode = fighter_loader
	for d in [%Attacker, %Defender]:
		d.set_essentials(CardImageLoaderNode, FighterImageLoaderNode, ColorManagerNode)

func load_match(data):
	if data.Combat == null:
		hide()
		return
	show()
	%Attacker.load_combat(data)
	%Defender.load_combat(data)
	
