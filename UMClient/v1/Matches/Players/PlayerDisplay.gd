extends PanelContainer

@export var inverted: bool = false

@onready var MainContainer = %Main

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	if inverted:
		var count = MainContainer.get_child_count()
		while count > 0:
			MainContainer.move_child(MainContainer.get_child(0), count - 1)
			count -= 1

	pass # Replace with function body.
