extends HBoxContainer
class_name CombatDisplay

@export var CardImageLoaderNode: CardImageLoader
@export var FighterImageLoaderNode: FighterImageLoader


func _ready() -> void:
	for d in [%Attacker, %Defender]:
		d.set_essentials(CardImageLoaderNode, FighterImageLoaderNode)

func load_match(data):
	if data.Combat == null:
		hide()
		return
	show()
	%Attacker.load_combat(data)
	%Defender.load_combat(data)
	
