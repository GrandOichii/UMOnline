extends PanelContainer
class_name EventListDisplay

@export var MaxDisplayed = -1
@export var EventToSceneMapping: Dictionary 

@export var CardImageLoaderNode: CardImageLoader
@export var FighterImageLoaderNode: FighterImageLoader
@export var ZoomedCardNode: ZoomedCard

@onready var EventsNode = %Events

func _ready() -> void:
	while EventsNode.get_child_count() > 0:
		EventsNode.remove_child(EventsNode.get_child(0))
	_scroll_to_bottom()

func _scroll_to_bottom():
	%Scroll.scroll_vertical = %Scroll.get_v_scroll_bar().max_value
	
func load_new_events(update_info):
	for e in update_info.NewEvents:
		# TODO check that exists
		var ps = EventToSceneMapping[e.EventType] as PackedScene
		var child = ps.instantiate() as EventDisplay
		
		EventsNode.add_child(child)
		
		child.set_essentials(CardImageLoaderNode, FighterImageLoaderNode, ZoomedCardNode.load_card)
		child.load_event(e)
