extends Control
class_name MapNodeDisplay

@export var NodeId: int
@export var DefaultBorderColor: Color
@export var PickableBorderColor: Color

func _ready() -> void:
	%ID.text = str(NodeId)


func _on_mouse_entered() -> void:
	print('ENTER' + str(NodeId))


func _on_mouse_exited() -> void:
	print('EXIT' + str(NodeId))
