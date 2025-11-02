class_name TokenDisplay
extends Control

var _token_loader: TokenImageLoader

func set_essentials(token_loader: TokenImageLoader):
	_token_loader = token_loader
	
func load(tokens: Array[String]):
	# TODO optimize
	while get_child_count() > 0:
		remove_child(get_child(0))
		
	for token in tokens:
		var node = TextureRect.new()
		add_child(node)
		node.expand_mode = TextureRect.EXPAND_IGNORE_SIZE
		node.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT_CENTERED
		
		node.texture = _token_loader.get_image_for(token)
