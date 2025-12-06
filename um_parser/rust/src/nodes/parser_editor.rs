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

    edited_parser_id: Option<i32>,
    pub project_name: Option<String>,

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

    pub fn clear(&mut self, project_name: String) {
        self.project_name = Some(project_name);
        self.edited_parser_id = None;
        self.name_edit.clear();
        self.type_picker.select(1);
        self.pattern_edit.clear();
        self.description_edit.clear();
        self.display_default_script();
    }

    pub fn load(&mut self, parser: &ParserModel) {
        self.project_name = Some(parser.project_name.to_string());
        self.edited_parser_id = Some(parser.id);
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
        self.on_type_picker_text_changed(-1);
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
            .get_parsers_with_name(self.project_name.as_ref().unwrap(), &parser.name)
            .expect("Failed to get parsers");
        let iter = binding.iter();
        let amount = match self.edited_parser_id {
            Some(id) => iter.filter(|p| p.id != id).count(),
            None => iter.count(),
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

        // TODO check that can extract _Create function
        match lua.load(&parser.script).exec() {
            Ok(_) => None,
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
        ParserModel {
            children: vec![],
            description: self.description_edit.get_text().to_string(),
            name: self.name_edit.get_text().to_string(),
            pattern: self.pattern_edit.to_string(),
            ptype: self.get_ptype(),
            script: self.script_edit.get_text().to_string(),
            id: match self.edited_parser_id {
                Some(id) => id,
                None => -1
            },

            editor_offset_x: 0.0, // TODO 
            editor_offset_y: 0.0, // TODO
            ref_to_id: None,
            is_root: false, // TODO
            is_template: false, // TODO
            parent_id: None, // TODO
            parent_slot: None, // TODO
            project_name: String::from(""), // TODO
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
