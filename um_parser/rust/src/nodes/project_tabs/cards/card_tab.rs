use godot::classes::*;
use godot::prelude::*;

use stylua_lib::Config;
use stylua_lib::OutputVerification;
use stylua_lib::format_code;

use crate::model::card::CardModel;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::script_display::ScriptDisplayNode;

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
    script_display: OnEditor<Gd<ScriptDisplayNode>>,
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
        let mut script = match ph {
            None => String::from(""),
            Some(history) => match history.get_script_for(self.card_id.unwrap()) {
                Some(script) => script.to_string(),
                None => String::from(""),
            },
        };

        // let formatted = format_code(&script, Config::default(), None, OutputVerification::None);
        // match formatted {
        //     Ok(new_script) => script = new_script,
        //     Err(err) => {
        //         // TODO
        //         godot_print!("{}", err);
        //     }
        // }
        self.script_display.bind_mut().set_script_text(&script);
    }
}
