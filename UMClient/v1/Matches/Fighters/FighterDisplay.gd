extends Control
class_name FighterDisplay

@onready var CurHealthLabel = %CurHealth
@onready var MaxHealthLabel = %MaxHealth
@onready var NameLabel = %Name

func load_fighter(data):
	CurHealthLabel.text = str(data.CurHealth)
	MaxHealthLabel.text = str(data.MaxHealth)
	NameLabel.text = data.Name
	%Image.load_fighter(data.Id, data.Name)
	# TODO use IsAlive

func set_essentials(fighter_image_loader: FighterImageLoader):
	%Image.set_essentials(fighter_image_loader)
