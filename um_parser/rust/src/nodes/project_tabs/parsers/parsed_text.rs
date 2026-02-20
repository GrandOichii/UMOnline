use godot::classes::*;
use godot::prelude::*;

use crate::model::card::CardModel;
use crate::nodes::parsing_history::ParsedText;
use crate::nodes::project_tabs::cards::cards_tab::CardsTabNode;
use crate::nodes::project_tabs::parsers::colored_text::ColoredTextNode;
use crate::nodes::project_tabs::parsers::text::TextNode;

#[derive(GodotClass)]
#[class(init,base=PanelContainer)]
pub struct ParsedTextNode {
    base: Base<PanelContainer>,

    card_id: Option<i32>,
    loaded_text: Option<ParsedText>,

    #[init(val = OnReady::manual())]
    pub cards_tab: OnReady<Gd<CardsTabNode>>,

    #[export_group(name = "Nodes")]
    #[export]
    original_text_label: OnEditor<Gd<Label>>,
    #[export]
    generated_text_label: OnEditor<Gd<Label>>,
    #[export]
    full_text_label: OnEditor<Gd<ColoredTextNode>>,
    #[export]
    card_ref_button: OnEditor<Gd<Button>>,
}

#[godot_api]
impl IPanelContainer for ParsedTextNode {
    fn ready(&mut self) {
        self.card_ref_button.hide();

        self.connect_signals();
    }
}

// private methods
impl ParsedTextNode {
    fn connect_signals(&mut self) {
        self.card_ref_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_card_ref_button_pressed);
    }
}

// signal connections
impl ParsedTextNode {
    fn on_card_ref_button_pressed(&mut self) {
        self.cards_tab.bind_mut().open_card(self.card_id.unwrap());
        self.cards_tab.set_visible(true);
    }
}

impl TextNode for ParsedTextNode {
    fn load_parsed_text(&mut self, parsed_text: &ParsedText, card: &CardModel) {
        self.original_text_label.set_text(&parsed_text.original);
        self.generated_text_label.set_text(&parsed_text.generated);
        self.full_text_label.bind_mut().load_text(
            parsed_text.full_text.to_string(),
            parsed_text.original.to_string(),
        );

        self.card_ref_button.set_text(card.name.as_str());

        self.card_id = Some(card.id);
        self.loaded_text = Some(ParsedText::from(parsed_text));
    }

    fn init_cards_tab(&mut self, cards_tab: Gd<CardsTabNode>) {
        self.card_ref_button.show();
        self.cards_tab.init(cards_tab);
    }

    fn get_text(&self) -> &ParsedText {
        self.loaded_text.as_ref().unwrap()
    }
}
