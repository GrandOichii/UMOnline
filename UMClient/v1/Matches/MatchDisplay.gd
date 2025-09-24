extends Control

@onready var PlayerNodes = [%P1, %P2, %P3, %P4]
@onready var MapNode = %Map

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

func load_match(data):
	# players
	for i in range(len(data.Players)):
		PlayerNodes[i].load_player(data, i)
	for i in range(len(data.Players), len(PlayerNodes)):
		PlayerNodes[i].visible = false
		
	# players
	MapNode.load_map(data)
