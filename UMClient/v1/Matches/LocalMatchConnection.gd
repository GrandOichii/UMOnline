extends MatchConnection
class_name LocalMatchConnection

signal responded(resp: String)

func respond(resp: String):
	responded.emit(resp)
