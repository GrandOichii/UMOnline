extends Control
class_name HandCardDisplay

@export var DefaultBorderColor: Color
@export var PickableBorderColor: Color

@onready var CardNode = %Card

var _cur_data
var _idx
var _connection: MatchConnection

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	reset_border_color()
	
func set_idx(idx):
	_idx = idx
	%IDX.text = str(idx)
	
func set_essentials(connection, image_loader, deck_name, on_load_card):
	_connection = connection
	CardNode.set_essentials(image_loader, deck_name, on_load_card)
	
func load_card(data):
	_cur_data = data
	if data != null:
		%ID.text = str(data.Id)
	CardNode.load_hand_card(data)
	
func can_pick():
	return _cur_data != null and _connection.can_pick_card_in_hand(_cur_data.Id)

func _on_card_mouse_entered() -> void:
	if not can_pick():
		return
	%Border.color = PickableBorderColor

func _on_card_mouse_exited() -> void:
	reset_border_color()
	
func reset_border_color():
	%Border.color = DefaultBorderColor

func _on_card_gui_input(event: InputEvent) -> void:
	if event.is_action_pressed("pick_card") and can_pick():
		_connection.pick_card_in_hand(_cur_data.Id)
		reset_border_color()
		return
