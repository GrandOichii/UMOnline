extends Control

@export var LogCardColor: Color

@onready var PlayerNodes = [%P1, %P2, %P3, %P4]
@onready var MapNode = %Map
@onready var CombatNode = %Combat
@onready var LogsNode = %Logs

@onready var ConnectionControlsNode = %ConnectionControls

@onready var _controls_map = {
	'ChooseString': %ChooseStringControls,
	'ChooseAction': %ChooseActionControls,
}

var _player_name_pattern = RegEx.new()
var _fighter_name_pattern = RegEx.new()
var _card_name_pattern = RegEx.new()


var _connection: MatchConnection = null
#var _fighter_image_loader = null

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ConnectionControlsNode.hide()
	LogsNode.clear()
	_player_name_pattern.compile(r'\((.+?):(.+?)\)')
	_fighter_name_pattern.compile(r'\[(.+?):(.+?)\]')
	_card_name_pattern.compile(r'\{(.+?):(.+?):(.+?)\}')

func _input(e: InputEvent) -> void:
	if _connection.can_pick_action():
		if e.is_action_pressed("fight"):
			_connection.respond('Fight')
			return
		if e.is_action_pressed("manoeuvre"):
			_connection.respond('Manoeuvre')
			return
		if e.is_action_pressed("scheme"):
			_connection.respond('Scheme')
			return

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
	%Colors.load_setup(setup_data)
	MapNode.load_setup(setup_data)
	for i in range(len(setup_data.Players)):
		PlayerNodes[i].load_setup(setup_data.Players[i])

func load_match(data):
	# players
	for i in range(len(data.Players)):
		PlayerNodes[i].load_player(data, i)
	for i in range(len(data.Players), len(PlayerNodes)):
		PlayerNodes[i].visible = false
		
	# map
	MapNode.load_map(data)
	
	# combat
	CombatNode.load_match(data)

func _replace_player_names(msg: String) -> String:
	var matches = _player_name_pattern.search_all(msg)
	for m in matches:
		var orig = m.strings[0]
		var idx = m.strings[1]
		var player_name = m.strings[2]
		var color = %Colors.get_player_color(idx)
		msg = msg.replace(orig, '[color=' + color.to_html() + ']' + player_name + '[/color]')
	return msg
	
func _replace_fighter_names(msg: String) -> String:
	var matches = _fighter_name_pattern.search_all(msg)
	for m in matches:
		var orig = m.strings[0]
		var idx = m.strings[1]
		var fighter_name = m.strings[2]
		var color = %Colors.get_fighter_color(idx)
		msg = msg.replace(orig, '[color=' + color.to_html() + ']' + fighter_name + '[/color]')
	return msg

func _replace_card_names(msg: String) -> String:
	var matches = _card_name_pattern.search_all(msg)
	for m in matches:
		var orig = m.strings[0]
		var key = m.strings[1]
		# var idx = m.strings[2]
		var card_name = m.strings[3]
		var color = LogCardColor
		msg = msg.replace(orig, '[color=' + color.to_html() + '][url="' + key + '"]' + card_name + '[/url][/color]')
		#msg = msg.replace(orig, '[color=' + color.to_html() + ']' + card_name + '[/color]')
		
	return msg

func _formatted_log_msg(msg: String) -> String:
	var result = msg
	result = _replace_fighter_names(result)
	result = _replace_player_names(result)
	result = _replace_card_names(result)

	return '- ' + result + '\n'

func _load_logs(new_logs):
	for newLog in new_logs:
		LogsNode.append_text(_formatted_log_msg(newLog.Message))
	
	LogsNode.scroll_to_line(LogsNode.get_line_count()-1)


func load_connected_match(update_info):
	load_match(update_info.Match)
	
	# events
	%Events.load_new_events(update_info)

	# logs
	_load_logs(update_info.NewLogs)
	
	# controls
	_hide_controls()
	%HintText.text = update_info.Hint
	
	if update_info.Request not in _controls_map:
		if update_info.Request == 'ChooseCardInHandOrNothing':
			%ChooseNothingInHandButton.show()
		return

	var control = _controls_map[update_info.Request]
	control.show()
	while control.get_child_count() > 0:
		control.remove_child(control.get_child(0))
	for key in update_info.Args:
		var node = Button.new()
		node.size_flags_horizontal = Control.SIZE_EXPAND_FILL
		node.text = update_info.Args[key]
		node.pressed.connect(func():
			send_response_from_controls(update_info.Args[key]))
		control.add_child(node)

func send_response_from_controls(resp: String):
	_connection.respond(resp)

func _on_choose_nothing_in_hand_button_pressed() -> void:
	_connection.respond('')

func _on_logs_meta_hover_started(meta: Variant) -> void:
	%ZoomedCard.load_card(meta)
