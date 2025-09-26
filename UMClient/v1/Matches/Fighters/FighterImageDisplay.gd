extends Control
class_name FighterImageDisplay

var _loader: FighterImageLoader

func set_essentials(loader: FighterImageLoader):
	_loader = loader

func load_fighter(fighter_id: int, fighter_key: String):
	%Image.texture = _loader.get_image_for(fighter_id, fighter_key)
