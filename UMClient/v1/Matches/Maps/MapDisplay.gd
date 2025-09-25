extends PanelContainer

@export var FighterScene: PackedScene

@onready var NodesNode = %Nodes
@onready var HiddenFightersNode = %HiddenFighters

var _node_map: Dictionary = {}
#var _fighter_map: Dictionary = {}

var fighter_nodes = []

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	_update_node_map()
	
func set_essentials(connection: MatchConnection):
	for child: MapNodeDisplay in NodesNode.get_children():
		child.set_essentials(connection)

func _update_node_map():
	for child in NodesNode.get_children():
		var node = child as MapNodeDisplay
		_node_map[str(node.NodeId)] = node

func load_map(match_data):
	_load_fighters(match_data)
	_load_nodes(match_data)
	
func _load_nodes(_match_data):
	pass

func _get_node(node_id):
	return _node_map[str(node_id)]
	
func _load_fighters(match_data):
	# remove dead fighters from the board
	# move existing fighters
	var fighters = []
	for node in match_data.Map.Nodes:
		if node.FighterId == null:
			continue
		fighters.push_back([node.FighterId, node.Id])
		
	var count = len(fighters)
	while len(fighter_nodes) > count:
		fighters.remove(fighter_nodes[0])
		fighter_nodes[0].free()
	while len(fighter_nodes) < count:
		var child = FighterScene.instantiate()
		fighter_nodes.push_back(child)
		HiddenFightersNode.add_child(child)
		
		# var display = child as FighterFigureDisplay
		# TODO
		#display.set_essentials(image_loader, _deck_name, ZoomedCardImage.load_card)
		
	for i in range(count):
		var pair = fighters[i]
		var _fighter_id = pair[0]
		var node_id = pair[1]
		var map_node = _get_node(node_id)
		var fighter_node = fighter_nodes[i] as FighterFigureDisplay
		# TODO load fighter data
		fighter_node.reparent(map_node, false)
		fighter_node.position.x = 0
		fighter_node.position.y = 0
		
		# fighter_node.reparent(map_node, false)
		#fighter_node.reparent(FightersNode)
		# fighter_node.set_position(map_node.position)
