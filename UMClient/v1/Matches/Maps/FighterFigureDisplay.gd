extends Control
class_name FighterFigureDisplay

@export var DefaultColor: Color
@export var PickableColor: Color
@export var HoverColor: Color

@onready var BorderNode = %Border

var _fighter
var _connection: MatchConnection

func _ready() -> void:
	BorderNode.hide()

func set_fighter(fighter):
	_fighter = fighter
	%Image.load_fighter(fighter.Id, fighter.Key)
	# TODO

func set_essentials(connection: MatchConnection, fighter_image_loader: FighterImageLoader):
	_connection = connection
	_connection.match_info_updated.connect(_on_match_info_updated)
	%Image.set_essentials(fighter_image_loader)

func get_id():
	return _fighter.Id

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

	
func _on_match_info_updated(match_info):
	if match_info.Request == 'ChooseFighter':
		call_deferred('show_border')
		return
	hide_border()

	
func show_border():
	if can_pick():
		BorderNode.show()
		BorderNode.modulate = PickableColor
		return
	BorderNode.modulate = DefaultColor

func hide_border():
	BorderNode.hide()

func can_pick():
	return _connection.can_pick_fighter(_fighter.Id)

func _on_gui_input(event: InputEvent) -> void:
	if can_pick() and event.is_action_pressed("pick_fighter"):
		_connection.pick_fighter(_fighter.Id)
		
