extends Control

@export var Connection: MatchConnection

@onready var Display = %Display

@onready var HintLabel = %HintLabel
@onready var RequestLabel = %RequestLabel
@onready var ResponseEdit = %ResponseEdit
@onready var OptionsDisplay = %OptionsDisplay

func _ready() -> void:
	Connection.match_info_updated.connect(_on_match_info_updated)
	Display.set_connection(Connection)
	
func _on_match_info_updated(data):
	if data.Request == 'Setup':
		Display.load_setup(data.Setup)
	Display.load_connected_match(data)

	HintLabel.text = data.Hint
	RequestLabel.text = data.Request
	OptionsDisplay.text = str(data.Args)


func _on_send_button_pressed() -> void:
	Connection.respond(ResponseEdit.text)
	ResponseEdit.clear()
