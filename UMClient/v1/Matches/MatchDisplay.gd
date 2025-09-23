extends Control

@onready var PlayerNodes = [%P1, %P2, %P3, %P4]

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.

func load(matchData):
	# players
	for i in range(len(matchData.Players)):
		PlayerNodes[i].load(matchData, i)
	for i in range(len(matchData.Players), len(PlayerNodes)):
		PlayerNodes[i].visible = false
