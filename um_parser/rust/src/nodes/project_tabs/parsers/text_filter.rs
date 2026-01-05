use godot::classes::*;
use godot::prelude::*;

#[derive(GodotClass)]
#[class(init,base=VBoxContainer)]
pub struct TextFilterNode {
    base: Base<VBoxContainer>,
    //#[export_group(name="Nodes")]
}

#[godot_api]
impl IVBoxContainer for TextFilterNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl TextFilterNode {
    fn connect_signals(&mut self) {}
}
