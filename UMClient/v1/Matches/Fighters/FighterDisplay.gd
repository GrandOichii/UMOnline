extends Control
class_name FighterDisplay

@onready var CurHealthLabel = %CurHealth
@onready var MaxHealthLabel = %MaxHealth
@onready var NameLabel = %Name

var _color_manager: ColorManager

func load_fighter(data):
	CurHealthLabel.text = str(int(data.CurHealth))
	MaxHealthLabel.text = str(int(data.MaxHealth))
	NameLabel.text = data.Name
	NameLabel.add_theme_color_override("font_color", _color_manager.get_fighter_color(data.Id))
	
	%Image.load_fighter(data.Id, data.Name)
	# TODO use IsAlive

func set_essentials(fighter_image_loader: FighterImageLoader, color_manager: ColorManager):
	%Image.set_essentials(fighter_image_loader)
	_color_manager = color_manager
