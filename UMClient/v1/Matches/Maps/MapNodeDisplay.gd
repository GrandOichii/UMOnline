extends Control
class_name MapNodeDisplay

@export var NodeId: int
@export var DefaultColor: Color
@export var PickableColor: Color
@export var HoverColor: Color

@onready var BorderNode = %Border
# @onready var BorderNode = %Border

@onready var FighterContainerNode = %FighterContainer

var _connection: MatchConnection

func _ready() -> void:
	BorderNode.hide()
	%ID.text = str(NodeId)

func _on_mouse_entered() -> void:
	if not can_pick():
		return
	BorderNode.modulate = HoverColor

func _on_mouse_exited() -> void:
	BorderNode.modulate = _default_color()
	
func _default_color():
	if can_pick():
		return PickableColor
	return DefaultColor
	
func set_essentials(connection: MatchConnection):
	_connection = connection
	_connection.match_info_updated.connect(_on_match_info_updated)
	
func _on_match_info_updated(match_info):
	if match_info.Request == 'ChooseNode':
		call_deferred('show_fill')
		return
	hide_fill()
	
func show_fill():
	if can_pick():
		BorderNode.show()
		BorderNode.modulate = PickableColor
		return
	BorderNode.modulate = DefaultColor

func hide_fill():
	BorderNode.hide()
	
func can_pick():
	return _connection.can_pick_node(NodeId)

func _on_gui_input(event: InputEvent) -> void:
	if can_pick() and event.is_action_pressed("pick_node"):
		_connection.pick_node(NodeId)
		
