use godot::classes::*;
use godot::prelude::*;

use crate::model::card::CardModel;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardTabNode {
    base: Base<Control>,
    #[export_group(name="Nodes")]
    #[export]
    name_label: OnEditor<Gd<Label>>,
    #[export]
    text_display: OnEditor<Gd<TextEdit>>,
}

#[godot_api]
impl IControl for CardTabNode {
    fn ready(&mut self) {}
}

impl CardTabNode {
    pub fn load_card(&mut self, card: &CardModel) {
        self.name_label.set_text(&card.name);
        self.text_display.set_text(&card.text);
    }
}
