use std::collections::HashSet;

use godot::classes::*;
use godot::prelude::*;

use crate::nodes::parsing_history::ParsedText;

#[derive(GodotClass)]
#[class(init,base=VBoxContainer)]
pub struct TextFilterNode {
    base: Base<VBoxContainer>,
    #[export_group(name = "Nodes")]
    #[export]
    text_filter: OnEditor<Gd<LineEdit>>,
    #[export]
    unique_check: OnEditor<Gd<CheckBox>>,
}

#[godot_api]
impl TextFilterNode {
    #[signal]
    pub fn filter_changed();
}

#[godot_api]
impl IVBoxContainer for TextFilterNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

// private methods
impl TextFilterNode {
    fn connect_signals(&mut self) {
        self.text_filter
            .signals()
            .text_changed()
            .connect_other(self, Self::on_text_filter_text_changed);
        self.unique_check
            .signals()
            .pressed()
            .connect_other(self, Self::on_unique_check_pressed);
    }

    fn change_filter(&mut self) {
        self.signals().filter_changed().emit();
    }
}

// public methods
impl TextFilterNode {
    pub fn create_text_filter_instance(&self) -> TextFilterInstance {
        TextFilterInstance {
            text_filter: self.text_filter.get_text().to_string(),
            unique_only: self.unique_check.is_pressed(),
            texts: HashSet::<String>::new(),
        }
    }
}

// signal connections
impl TextFilterNode {
    fn on_unique_check_pressed(&mut self) {
        self.change_filter();
    }

    fn on_text_filter_text_changed(&mut self, _: GString) {
        self.change_filter();
    }
}

// TextFilterInstance

pub struct TextFilterInstance {
    text_filter: String,
    unique_only: bool,
    texts: HashSet<String>,
}

// public methods
impl TextFilterInstance {
    pub fn check(&mut self, text: &ParsedText) -> bool {
        if self.unique_only && self.texts.contains(&text.original) {
            return false;
        }

        self.texts.insert(text.original.to_string());
        let contains = !text
            .original
            .to_lowercase()
            .contains(&self.text_filter.to_lowercase());
        godot_print!(
            "{} CONTAINS {} -> {}",
            &text.original,
            &self.text_filter,
            contains
        );
        if contains {
            return false;
        }

        return true;
    }
}
