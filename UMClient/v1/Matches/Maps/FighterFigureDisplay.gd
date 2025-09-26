extends Control
class_name FighterFigureDisplay

var _fighter

func set_fighter(fighter):
	_fighter = fighter
	%Image.load_fighter(fighter.Id, fighter.Key)
	# TODO

func set_essentials(connection: MatchConnection, fighter_image_loader: FighterImageLoader):
	%Image.set_essentials(fighter_image_loader)

func get_id():
	return _fighter.Id
