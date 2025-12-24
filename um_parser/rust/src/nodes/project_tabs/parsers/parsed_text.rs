use godot::classes::*;
use godot::prelude::*;

use crate::nodes::parsing_history::ParsedText;

#[derive(GodotClass)]
#[class(init,base=PanelContainer)]
pub struct ParsedTextNode {
    base: Base<PanelContainer>,

    #[export_group(name="Nodes")]
    #[export]
    original_text_label: OnEditor<Gd<Label>>,
    #[export]
    generated_text_label: OnEditor<Gd<Label>>,
    #[export]
    full_text_label: OnEditor<Gd<Label>>,
    #[export]
    card_ref_button: OnEditor<Gd<Button>>,
}

#[godot_api]
impl IPanelContainer for ParsedTextNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl ParsedTextNode {
    fn connect_signals(&mut self) {
        self.card_ref_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_card_ref_button_pressed);
    }

    fn on_card_ref_button_pressed(&mut self) {
        // TODO open cards tab, open specific referenced card
    }

    pub fn load_parsed_text(&mut self, parsed_text: &ParsedText) {
        self.original_text_label.set_text(&parsed_text.original);
        self.generated_text_label.set_text(&parsed_text.generated);
        self.full_text_label.set_text(&parsed_text.full_text);

        // TODO card_id
    }
}