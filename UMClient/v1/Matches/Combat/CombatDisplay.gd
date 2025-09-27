extends HBoxContainer
class_name CombatDisplay

@export var CardImageLoaderNode: CardImageLoader
@export var FighterImageLoaderNode: FighterImageLoader
@export var ColorManagerNode: ColorManager
@export var ZoomedCardNode: ZoomedCard

func _ready() -> void:
	set_essentials(CardImageLoaderNode, FighterImageLoaderNode, ZoomedCardNode)
		
func set_essentials(card_loader: CardImageLoader, fighter_loader: FighterImageLoader, zoomed_card: ZoomedCard):
	CardImageLoaderNode = card_loader
	FighterImageLoaderNode = fighter_loader
	ZoomedCardNode = zoomed_card
	var load_card = null
	if ZoomedCardNode != null:
		load_card = ZoomedCardNode.load_card
	for d in [%Attacker, %Defender]:
		d.set_essentials(CardImageLoaderNode, FighterImageLoaderNode, ColorManagerNode, load_card)

func load_match(data):
	if data.Combat == null:
		hide()
		return
	show()
	%Attacker.load_combat(data)
	%Defender.load_combat(data)
	
