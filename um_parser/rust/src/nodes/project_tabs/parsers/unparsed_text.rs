use godot::classes::*;
use godot::prelude::*;

use crate::model::card::CardModel;
use crate::nodes::parsing_history::ParsedText;
use crate::nodes::project_tabs::cards::cards_tab::CardsTabNode;
use crate::nodes::project_tabs::parsers::colored_text::ColoredTextNode;

#[derive(GodotClass)]
#[class(init,base=PanelContainer)]
pub struct UnparsedTextNode {
    base: Base<PanelContainer>,

    card_id: Option<i32>,

    #[init(val = OnReady::manual())]
    pub cards_tab: OnReady<Gd<CardsTabNode>>,

    #[export_group(name = "Nodes")]
    #[export]
    text_label: OnEditor<Gd<ColoredTextNode>>,
    #[export]
    card_ref_button: OnEditor<Gd<Button>>,
}

#[godot_api]
impl IPanelContainer for UnparsedTextNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl UnparsedTextNode {
    fn connect_signals(&mut self) {
        self.card_ref_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_card_ref_button_pressed);
    }

    fn on_card_ref_button_pressed(&mut self) {
        self.cards_tab.bind_mut().open_card(self.card_id.unwrap());
        self.cards_tab.set_visible(true);
    }

    pub fn load_parsed_text(&mut self, parsed_text: &ParsedText, card: &CardModel) {
        self.text_label.bind_mut().load_text(
            parsed_text.full_text.to_string(),
            parsed_text.original.to_string(),
        );

        self.card_ref_button.set_text(card.name.as_str());

        self.card_id = Some(card.id);
    }
}
