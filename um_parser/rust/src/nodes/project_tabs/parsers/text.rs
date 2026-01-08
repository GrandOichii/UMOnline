use godot::obj::Gd;

use crate::{model::card::CardModel, nodes::{parsing_history::ParsedText, project_tabs::cards::cards_tab::CardsTabNode}};

pub trait TextNode {
    fn load_parsed_text(&mut self, parsed_text: &ParsedText, card: &CardModel);
    fn init_cards_tab(&mut self, cards_tab: Gd<CardsTabNode>);
    fn get_text(&self) -> &ParsedText;
}
