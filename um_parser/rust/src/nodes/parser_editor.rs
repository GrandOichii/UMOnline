use godot::classes::*;
use godot::prelude::*;
use mlua::Lua;
use regex::Regex;

use crate::model::parser::*;
use crate::parsers::selector::SELECTOR_SCRIPT;
use crate::parsers::splitter::SPLITTER_SCRIPT;

#[derive(GodotClass)]
#[class(init,base=Window)]
pub struct ParserEditorWindowNode {
    base: Base<Window>,

    #[export_group(name = "Nodes")]
    #[export]
    name_edit: OnEditor<Gd<LineEdit>>,
    #[export]
    type_picker: OnEditor<Gd<OptionButton>>,
    #[export]
    pattern_container: OnEditor<Gd<Container>>,
    #[export]
    pattern_edit: OnEditor<Gd<LineEdit>>,
    #[export]
    description_edit: OnEditor<Gd<TextEdit>>,
    #[export]
    script_edit: OnEditor<Gd<CodeEdit>>,
    #[export]
    save_button: OnEditor<Gd<Button>>,
    #[export]
    cancel_button: OnEditor<Gd<Button>>,
    #[export]
    save_error_dialog: OnEditor<Gd<AcceptDialog>>,
    #[export]
    save_error_reasons_label: OnEditor<Gd<Label>>,
}


#[godot_api]
impl ParserEditorWindowNode {
    #[signal]
    pub fn save_request();
    #[signal]
    pub fn cancel_request();
}

#[godot_api]
impl IWindow for ParserEditorWindowNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.save_error_dialog.hide();
        self.on_type_picker_text_changed(0);
    }
}

impl ParserEditorWindowNode {
    fn connect_signals(&mut self) {
        self.save_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_save_request);
        self.cancel_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_cancel_button_pressed);
        self.type_picker
            .signals()
            .item_selected()
            .connect_other(self, Self::on_type_picker_text_changed);
    }

    fn on_cancel_button_pressed(&mut self) {
        self.base_mut().hide();
        self.signals().cancel_request().emit();
    }

    pub fn clear(&mut self) {
        self.name_edit.clear();
        self.type_picker.select(1);
        self.pattern_edit.clear();
        self.description_edit.clear();
        self.display_default_script();
    }

    fn display_default_script(&mut self) {
        let ptype = self.get_ptype();
        self.script_edit.set_text(match ptype {
            PMT_MATCHER => {
                "function _Create(text, children, data)
    return 'TODO'
end"
            }
            PMT_SELECTOR => SELECTOR_SCRIPT,
            PMT_SPLITTER => SPLITTER_SCRIPT,
            _ => panic!("Unrecognized ptype: {}", ptype),
        });
    }

    fn on_type_picker_text_changed(&mut self, _v: i64) {
        let ptype = self.get_ptype();
        self.pattern_container.set_visible(match ptype {
            PMT_SELECTOR => false,
            PMT_MATCHER => true,
            PMT_SPLITTER => true,
            _ => panic!("Unrecognized ptype: {}", ptype),
        });
        self.display_default_script();
    }

    fn on_save_request(&mut self) {
        let parser = self.build();
        let checks: Vec<fn(parser: &ParserModel) -> Option<String>> = vec![
            // empty name check
            |parser: &ParserModel| -> Option<String> {
                match parser.name.len() {
                    0 => Some(String::from("Name can't be empty")),
                    _ => None,
                }
            },
            // pattern check
            |parser: &ParserModel| -> Option<String> {
                match Regex::new(&parser.pattern) {
                    Ok(_) => None,
                    Err(_) => Some(String::from("Failed to compile pattern")),
                    // Err(err) => Some(format!("Failed to compile pattern: {:?}", err)),
                }
            },
            // lua script check
            |parser: &ParserModel| -> Option<String> {
                let lua = Lua::new();
                // lua
                match lua.load(&parser.script).exec() {
                    Ok(_) => None,
                    // Err(err) => Some(format!("Lua parse error: {:?}", err)),
                    Err(_) => Some(String::from("Lua parse error")),
                }
            },
        ];

        let mut errors = vec![];
        for check in checks {
            match check(&parser) {
                None => continue,
                Some(err) => errors.push(err),
            }
        }

        if errors.len() > 0 {
            self.save_error_reasons_label
                .set_text(errors.join("\n").as_str());
            self.save_error_dialog.show();
            return;
        }

        self.signals().save_request().emit();
    }

    pub fn build(&self) -> ParserModel {
        ParserModel {
            children: vec![],
            description: self.description_edit.get_text().to_string(),
            editor_offset_x: 0.0,
            editor_offset_y: 0.0,
            id: 0,
            is_ref: false,
            is_root: false,
            is_template: false,
            name: self.name_edit.get_text().to_string(),
            parent_id: None,
            pattern: self.pattern_edit.to_string(),
            project_name: String::from(""),
            ptype: self.get_ptype(),
            script: self.script_edit.get_text().to_string(),
        }
    }

    fn get_ptype(&self) -> ParserModelType {
        match self.type_picker.get_text().to_string().as_str() {
            "Matcher" => PMT_MATCHER,
            "Selector" => PMT_SELECTOR,
            "Splitter" => PMT_SPLITTER,
            other => panic!("Unrecognized parser model type was picked: {}", other),
        }
    }
}

// trait SaveCheck {

// }
