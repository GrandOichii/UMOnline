extends Control
class_name FighterDisplay

@onready var CurHealthLabel = %CurHealth
@onready var MaxHealthLabel = %MaxHealth
@onready var NameLabel = %Name

func load_fighter(data):
	CurHealthLabel.text = str(data.CurHealth)
	MaxHealthLabel.text = str(data.MaxHealth)
	NameLabel.text = data.Name
	# TODO use IsAlive
