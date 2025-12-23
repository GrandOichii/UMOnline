use godot::classes::*;
use godot::prelude::*;
use mlua::Lua;
use regex::Regex;

use crate::model::parser::*;
use crate::parsers::selector::SELECTOR_SCRIPT;
use crate::parsers::splitter::SPLITTER_SCRIPT;
use crate::repo::ParserRepositoryNode;

#[derive(GodotClass)]
#[class(init,base=Window)]
pub struct ParserEditorWindowNode {
    base: Base<Window>,

    edited_parser: Option<ParserModel>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,

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
            .connect_other(self, Self::cancel);
        self.type_picker
            .signals()
            .item_selected()
            .connect_other(self, Self::on_type_picker_text_changed);
        self.base()
            .signals()
            .close_requested()
            .connect_other(self, Self::cancel);
    }

    fn cancel(&mut self) {
        self.base_mut().hide();
        self.signals().cancel_request().emit();
    }

    pub fn clear(&mut self, project_name: String, is_template: bool) {
        self.type_picker.select(1);
        self.load(ParserModel {
            children: vec![],
            description: String::from(""),
            editor_offset_x: 0.0,
            editor_offset_y: 0.0,
            id: -1,
            is_template: is_template,
            name: String::from(""),
            parent_id: None,
            parent_slot: None,
            pattern: String::from(""),
            project_name: project_name,
            ptype: 2,
            ref_to_id: None,
            script: String::from(""),
            ref_name: None,
        });
        self.display_default_script();
    }

    pub fn load(&mut self, parser: ParserModel) {
        self.name_edit.set_text(&parser.name);
        self.type_picker.select(match parser.ptype {
            PMT_MATCHER => 0,
            PMT_SELECTOR => 1,
            PMT_SPLITTER => 2,
            other => panic!("Unrecognized parser type: {}", other),
        });
        self.pattern_edit.set_text(&parser.pattern);
        self.description_edit.set_text(&parser.description);
        self.script_edit.set_text(&parser.script);
        self.check_pattern_visibility();
        self.edited_parser = Some(parser);
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

    fn check_pattern_visibility(&mut self) {
        let ptype = self.get_ptype();
        self.pattern_container.set_visible(match ptype {
            PMT_SELECTOR => false,
            PMT_MATCHER => true,
            PMT_SPLITTER => true,
            _ => panic!("Unrecognized ptype: {}", ptype),
        });

    }

    fn on_type_picker_text_changed(&mut self, _v: i64) {
        self.check_pattern_visibility();
        self.display_default_script();
    }

    fn parser_empty_name_check(&mut self, parser: &ParserModel) -> Option<String> {
        match parser.name.len() {
            0 => Some(String::from("Name can't be empty")),
            _ => None,
        }
    }

    fn parser_taken_name_check(&mut self, parser: &ParserModel) -> Option<String> {
        let binding = self
            .repo
            .bind_mut()
            .get_parsers_with_name(&self.edited_parser.as_ref().unwrap().project_name, &parser.name)
            .expect("Failed to get parsers");
        let iter = binding.iter();
        let amount = match self.edited_parser.as_ref().unwrap().id {
            -1 => iter.count(),
            id => iter.filter(|p| p.id != id).count(),
        };

        match amount {
            0 => None,
            _ => Some(String::from("Parser with that name already exists")),
        }
    }

    fn parser_pattern_check(&mut self, parser: &ParserModel) -> Option<String> {
        match Regex::new(&parser.pattern) {
            Ok(_) => None,
            Err(_) => Some(String::from("Failed to compile pattern")),
            // Err(err) => Some(format!("Failed to compile pattern: {:?}", err)),
        }
    }

    fn parser_script_check(&mut self, parser: &ParserModel) -> Option<String> {
        let lua = Lua::new();

        match lua.load(&parser.script).exec() {
            Ok(_) => {
                match lua.globals().get::<mlua::Function>("_Create") {
                    Ok(_) => None,
                    Err(_) => Some(String::from("Didn't find _Create function"))
                }
            },
            // Err(err) => Some(format!("Lua parse error: {:?}", err)),
            Err(_) => Some(String::from("Lua parse error")),
        }
    }

    fn on_save_request(&mut self) {
        let parser = self.build();
        let checks = vec![
            ParserEditorWindowNode::parser_empty_name_check,
            ParserEditorWindowNode::parser_taken_name_check,
            ParserEditorWindowNode::parser_pattern_check,
            ParserEditorWindowNode::parser_script_check,
        ];

        let mut errors = vec![];
        for check in checks {
            match check(self, &parser) {
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
        let edited = self.edited_parser.as_ref().unwrap();
        ParserModel {
            children: vec![],
            description: self.description_edit.get_text().to_string(),
            name: self.name_edit.get_text().to_string(),
            pattern: self.pattern_edit.get_text().to_string(),
            ptype: self.get_ptype(),
            script: self.script_edit.get_text().to_string(),
            id: edited.id,
            editor_offset_x: edited.editor_offset_x,
            editor_offset_y: edited.editor_offset_y,
            ref_to_id: None,
            is_template: edited.is_template,
            parent_id: edited.parent_id,
            parent_slot: edited.parent_slot,
            project_name: edited.project_name.to_string(),
            ref_name: None,
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
