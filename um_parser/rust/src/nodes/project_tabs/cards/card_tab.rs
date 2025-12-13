use dprint_core::configuration::resolve_global_config;
use dprint_core::formatting;
use dprint_core::formatting::PrintItem;
use dprint_core::formatting::PrintItems;
use dprint_core::formatting::PrintOptions;
use godot::classes::*;
use godot::prelude::*;

use dprint_core::configuration::{ConfigKeyMap, ConfigKeyValue, GlobalConfiguration};

use crate::model::card::CardModel;
use crate::nodes::parsing_history::ParsingHistory;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardTabNode {
    base: Base<Control>,

    card_id: Option<i32>,

    #[export_group(name = "Nodes")]
    #[export]
    name_label: OnEditor<Gd<Label>>,
    #[export]
    text_display: OnEditor<Gd<TextEdit>>,
    #[export]
    script_display: OnEditor<Gd<CodeEdit>>,
}

#[godot_api]
impl IControl for CardTabNode {
    fn ready(&mut self) {}
}

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

        // let config = ConfigKeyMap::new();
        // let global_config = GlobalConfiguration::default();
        // // let config = resolve_global_config(global_);
        // let result = format_text()

        // let request = FormatRequest {
        //     file_text: script.to_string(),
        // };
        // formatting::format(|| {
        //     let result = PrintItems::new();
        //     result.push_item(PrintItem);
        //     script
        // }, PrintOptions {
        //     indent_width: 4,
        //     max_width: 10,
        //     use_tabs: false,
        //     // newline_kind: "\n",
        // })

        self.script_display.set_text(&script);
    }
}
