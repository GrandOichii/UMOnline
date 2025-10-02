extends PanelContainer

@export var FighterScene: PackedScene
@export var FighterImageLoaderNode: FighterImageLoader

@onready var NodesNode = %Nodes
@onready var HiddenFightersNode = %HiddenFighters

var _node_map: Dictionary = {}
#var _fighter_map: Dictionary = {}

var _connection
var _fighter_map = {}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_update_node_map()
	
func load_setup(match_data):
	for player in match_data.Players:
		for fighter in player.Fighters:
			var child: FighterFigureDisplay = FighterScene.instantiate()
			_fighter_map[fighter.Id] = child
			HiddenFightersNode.add_child(child)
			child.set_essentials(_connection, FighterImageLoaderNode)
			child.set_fighter(fighter)
	
func set_essentials(connection: MatchConnection):
	_connection = connection
	for child: MapNodeDisplay in NodesNode.get_children():
		child.set_essentials(connection)
	for key in _fighter_map:
		var node = _fighter_map[key]
		node.set_essentials(connection, FighterImageLoaderNode)

func _update_node_map():
	for child in NodesNode.get_children():
		var node = child as MapNodeDisplay
		_node_map[int(node.NodeId)] = node

func load_map(match_data):
	_load_fighters(match_data)
	_load_nodes(match_data)
	
func _load_nodes(_match_data):
	pass

func _get_node(node_id):
	return _node_map[int(node_id)]

func _get_fighter(fighter_id):
	return _fighter_map[fighter_id]

func _load_fighters(match_data):
	var missing_fighter_ids = []
	for id in _fighter_map:
		missing_fighter_ids.push_back(id)

	var fighters = []
	for node in match_data.Map.Nodes:
		if node.FighterId == null:
			continue
		# TODO
		#missing_fighter_ids.remove(node.FighterId)
		fighters.push_back([node.FighterId, node.Id])

	for id in missing_fighter_ids:
		var node = _get_fighter(id)
		node.reparent(HiddenFightersNode)

	for i in range(len(fighters)):
		var pair = fighters[i]
		var _fighter_id = pair[0]
		var node_id = pair[1]
		var map_node = _get_node(node_id)
		var fighter_node = _fighter_map[_fighter_id] as FighterFigureDisplay
		fighter_node.reparent(map_node.FighterContainerNode, false)
		#fighter_node.create_tween().tween_property(fighter_node, 'global_position', map_node.global_position, .5)
		fighter_node.position.x = 0
		fighter_node.position.y = 0
		
		# fighter_node.reparent(map_node, false)
		#fighter_node.reparent(FightersNode)
		# fighter_node.set_position(map_node.position)
