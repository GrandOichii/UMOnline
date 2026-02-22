use godot::classes::*;
use godot::prelude::*;

use crate::model::card::CardModel;
use crate::nodes::parsing_history::ParsedText;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::project_tabs::parsers::parsed_text::ParsedTextNode;
use crate::nodes::project_tabs::parsers::text::TextNode;
use crate::nodes::script_display::ScriptDisplayNode;
use crate::parsers::parser::ParseResult;
use crate::parsers::parser::ParseResultStatus;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardTabNode {
    base: Base<Control>,

    card_id: Option<i32>,

    #[export_group(name = "Packed scenes")]
    #[export]
    parsed_text_scene: OnEditor<Gd<PackedScene>>,

    #[export_group(name = "Nodes")]
    #[export]
    name_label: OnEditor<Gd<Label>>,
    #[export]
    text_display: OnEditor<Gd<TextEdit>>,
    #[export]
    script_display: OnEditor<Gd<ScriptDisplayNode>>,
    #[export]
    parsed_container: OnEditor<Gd<Container>>,
}

#[godot_api]
impl IControl for CardTabNode {
    fn ready(&mut self) {}
}

// public methods
impl CardTabNode {
    pub fn load_card(&mut self, card: &CardModel) {
        self.card_id = Some(card.id);
        self.name_label.set_text(&card.name);
        self.text_display.set_text(&card.text);
    }

    pub fn update_parsing_history(&mut self, ph: Option<&ParsingHistory>) {
        let script = match ph {
            None => String::from(""),
            Some(history) => match history.get_script_for(self.card_id.unwrap()) {
                Some(script) => script.to_string(),
                None => String::from(""),
            },
        };

        self.script_display.bind_mut().set_script_text(&script);

        self.update_parsed_container(ph);
    }
}

// private methods
impl CardTabNode {
    fn update_parsed_container(&mut self, ph_o: Option<&ParsingHistory>) {
        // remove old texts
        while self.parsed_container.get_child_count() > 0
            && let Some(node) = self.parsed_container.get_child(0)
        {
            self.parsed_container.remove_child(&node);
        }

        // load parsed texts
        let ph = match ph_o {
            Some(v) => v,
            None => return
        };
        
        let pr_o = ph.get_parse_result_for_card(self.card_id.unwrap());
        let pr = match pr_o {
            Some(v) => v,
            None => return
        };

        self.add_parsed_text(pr, &pr.text);
    }

    fn add_parsed_text(&mut self, pr: &ParseResult, full_text: &String) {
        if pr.status != ParseResultStatus::Success {
            return;
        }

        let mut node = self.parsed_text_scene.instantiate_as::<ParsedTextNode>();
        self.parsed_container.add_child(&node);
        // node.bind_mut().init_cards_tab(self.cards_tab.clone());

        node.bind_mut().load_parsed_text(&ParsedText {
            card_id: self.card_id.unwrap(),
            original: pr.text.to_string(),
            generated: pr.generated.to_string(),
            full_text: full_text.to_string()
        }, &CardModel {
            id: -1,
            name: String::from(""),
            text: String::from(""),
            project_name: String::from(""),
            used: true,
        });

        for child in pr.children.iter() {
            self.add_parsed_text(child, full_text);
        }
    }
}
