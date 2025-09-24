extends PanelContainer

@export var FighterScene: PackedScene

@onready var NodesNode = %Nodes
@onready var FightersNode = %Fighters

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_update_node_map()

func _update_node_map():
	pass

func load_map(match_data):
	# load nodes
	
	
	# load fighters
	pass
