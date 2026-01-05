use godot::prelude::*;

pub mod model;
pub mod nodes;
pub mod parsers;
pub mod repo;
pub mod traits;

struct UMParserExtension;

#[gdextension]
unsafe impl ExtensionLibrary for UMParserExtension {}
