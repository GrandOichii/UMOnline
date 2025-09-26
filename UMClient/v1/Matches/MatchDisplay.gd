extends Control

@onready var PlayerNodes = [%P1, %P2, %P3, %P4]
@onready var MapNode = %Map

@onready var ConnectionControlsNode = %ConnectionControls

@onready var _controls_map = {
	'ChooseString': [ %ChooseStringControls, %ChooseStringText, %ChooseStringOptions ],
	'ChooseAction': [ %ChooseActionControls, %ChooseActionText, %ChooseActionOptions ],
	
}

var _connection: MatchConnection = null
var _fighter_image_loader = null

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ConnectionControlsNode.hide()

func _hide_controls():
	for child in ConnectionControlsNode.get_children():
		child.hide()
	
func set_connection(connection: MatchConnection):
	_connection = connection
	ConnectionControlsNode.show()
	for pn in PlayerNodes:
		pn.set_essentials(_connection)
	# TODO
	
	MapNode.set_essentials(connection)

func load_setup(setup_data):
	MapNode.load_setup(setup_data)
	for i in range(len(setup_data.Players)):
		PlayerNodes[i].load_setup(setup_data.Players[i])

func load_match(data):
	# players
	for i in range(len(data.Players)):
		PlayerNodes[i].load_player(data, i)
	for i in range(len(data.Players), len(PlayerNodes)):
		PlayerNodes[i].visible = false
		
	# players
	MapNode.load_map(data)

func load_connected_match(update_info):
	load_match(update_info.Match)

	# controls
	_hide_controls()
	if update_info.Request not in _controls_map:
		if update_info.Request == 'ChooseCardInHandOrNothing':
			%ChooseNothingInHandButton.show()
		return

	var control = _controls_map[update_info.Request]
	control[0].show()
	control[1].text = update_info.Hint
	while control[2].get_child_count() > 0:
		control[2].remove_child(control[2].get_child(0))
	for key in update_info.Args:
		var node = Button.new()
		node.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		node.text = update_info.Args[key]
		node.pressed.connect(func():
			send_response_from_controls(update_info.Args[key]))
		control[2].add_child(node)

func send_response_from_controls(resp: String):
	_connection.respond(resp)

func _on_choose_nothing_in_hand_button_pressed() -> void:
	_connection.respond('')
