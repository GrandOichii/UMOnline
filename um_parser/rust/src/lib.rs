use godot::prelude::*;
use godot::classes::*;

pub mod parsers;
pub mod nodes;
pub mod model;
pub mod traits;
pub mod repo;

struct UMParserExtension;

#[gdextension]
unsafe impl ExtensionLibrary for UMParserExtension {}