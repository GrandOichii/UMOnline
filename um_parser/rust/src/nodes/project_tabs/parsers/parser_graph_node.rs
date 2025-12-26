use godot::classes::*;
use godot::prelude::*;
use regex::Regex;

use crate::model::parser::*;
use crate::nodes::parsing_history::ParserParsingHistory;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::project_tabs::parsers::parser_tab::ParserTabNode;
use crate::nodes::project_tabs::parsers::parsers_tab::ParsersTabNode;

struct ParserNodeBrief {
    pub pattern: String,
    pub parsed_count: usize,
    pub unparsed_count: usize,
}

impl ParserNodeBrief {
    pub fn get_completion_string(&self) -> String {
        format!(
            "{}/{}",
            self.parsed_count,
            self.unparsed_count + self.parsed_count
        )
    }
}

impl Default for ParserNodeBrief {
    fn default() -> Self {
        Self {
            pattern: String::from(""),
            parsed_count: 0,
            unparsed_count: 0,
        }
    }
}

#[derive(GodotClass)]
#[class(init,base=GraphNode)]
pub struct ParserGraphNode {
    base: Base<GraphNode>,

    brief: ParserNodeBrief,
    pub parser: Option<ParserModel>,

    #[init(val = OnReady::manual())]
    pub graph: OnReady<Gd<GraphEdit>>,
    #[init(val = OnReady::manual())]
    pub parsers_tab: OnReady<Gd<ParsersTabNode>>,
    #[init(val = OnReady::manual())]
    pub parent: OnReady<Gd<ParserTabNode>>,

    #[export]
    template_color: Color,
    #[export]
    local_color: Color,
    #[export]
    ref_color: Color,

    #[export_group(name = "Nodes")]
    #[export]
    completion_label: OnEditor<Gd<Label>>,
}

#[godot_api]
impl IGraphNode for ParserGraphNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl ParserGraphNode {
    fn connect_signals(&mut self) {
        self.base()
            .signals()
            .gui_input()
            .connect_other(self, Self::on_gui_input);
    }

    fn on_gui_input(&mut self, e: Gd<InputEvent>) {
        if e.is_action_pressed("edit_parser") {
            if let Some(ref_id) = self.parser.as_ref().unwrap().ref_to_id {
                self.parsers_tab.bind_mut().open_template(ref_id);
                return;
            }
            let editor_window = &mut self.parsers_tab.bind_mut().parser_editor_window;
            editor_window.bind_mut().mode =
                Some(crate::nodes::parser_editor::ParserEditorWindowNodeMode::Edit);

            editor_window
                .bind_mut()
                .load(self.parser.as_ref().unwrap().clone());
            editor_window.set_title(
                &format!("Edit parser {}", &self.parser.as_ref().unwrap().name).to_string(),
            );
            editor_window.show();
            return;
        }
        if e.is_action_pressed("display_parser_info") {
            self.parent
                .bind_mut()
                .load_parser_info(self.parser.as_ref().unwrap());
            return;
        }
    }

    pub fn load_parser(&mut self, parser: ParserModel) {
        // set color
        self.set_color(&parser);
        self.add_connection_slots(&parser);

        self.base_mut()
            .set_position_offset(Vector2::new(parser.editor_offset_x, parser.editor_offset_y));

        self.update_title(&parser);

        // additional type-specific control nodes
        // TODO this will be readded when calling load_parser again
        if parser.ref_to_id.is_none() {
            match parser.ptype {
                PMT_SELECTOR => {
                    let mut add_slot_button = Button::new_alloc();
                    add_slot_button.set_text("Add");
                    self.base_mut().add_child(&add_slot_button);
                    add_slot_button
                        .signals()
                        .pressed()
                        .connect_other(self, Self::on_add_slot_button_pressed);
                },
                _ => {}         
            };
        }

        self.parser = Some(parser);
        self.update_pattern();
    }

    fn get_slot_count(&mut self) -> i32 {
        let count = self.base_mut().get_child_count();
        let mut result: i32 = 0;
        for i in 0..count {
            if self.base_mut().is_slot_enabled_right(i) {
                result += 1;
            }
        }
        return result;
    }

    fn create_out_slot(&mut self) {
        let new_label = Label::new_alloc();
        self.base_mut().add_child(&new_label);
        self.base_mut().move_child(&new_label, 1);
        let idx = self.base_mut().get_child_count() - 2;
        self.base_mut().set_slot_enabled_right(idx, true);
    }

    fn on_add_slot_button_pressed(&mut self) {
        self.create_out_slot();
    }

    fn update_title(&mut self, parser: &ParserModel) {
        self.base_mut().set_title(match &parser.ref_name {
            Some(ref_name) => &ref_name,
            None => &parser.name,
        });
    }

    pub fn load_parsing_history(&mut self, ph: Option<&ParsingHistory>) {
        let binding = ParserParsingHistory {
            parsed_texts: vec![],
            unparsed_texts: vec![],
        };
        let name = &self.parser.as_ref().unwrap().name;
        let v = match ph {
            Some(p) => p.get_for(name).unwrap_or(&binding),
            None => &binding,
        };

        self.brief.parsed_count = v.parsed_texts.len();
        self.brief.unparsed_count = v.unparsed_texts.len();
        self.update_brief();
    }

    fn set_self_color(&mut self, color: Color) {
        self.base_mut().set_self_modulate(color);
    }

    fn set_color(&mut self, parser: &ParserModel) {
        if parser.ref_to_id.is_some() {
            self.set_self_color(self.ref_color);
            return;
        }
        if parser.is_template {
            self.set_self_color(self.template_color);
            return;
        }
        self.set_self_color(self.local_color);
    }

    fn add_connection_slots(&mut self, parser: &ParserModel) {
        self.add_in_connection_slot(parser);

        if parser.ref_to_id.is_some() {
            return;
        }

        match parser.ptype {
            PMT_MATCHER => self.add_matcher_connection_slots(parser),
            PMT_SELECTOR => self.add_selector_connection_slots(),
            PMT_SPLITTER => self.add_splitter_connection_slots(),
            other => panic!("Unrecognized parser model type: {}", other),
        }
    }

    fn add_in_connection_slot(&mut self, parser: &ParserModel) {
        if parser.is_template {
            return;
        }

        self.base_mut().set_slot_enabled_left(0, true);
    }

    fn add_matcher_connection_slots(&mut self, parser: &ParserModel) {
        let re = Regex::new(&parser.pattern).expect("Faield to parse regex");
        let capture_count = re.capture_locations().len();
        if capture_count == 0 {
            return;
        }
        for i in 0..capture_count - 1 {
            if i != 0 {
                let node = Label::new_alloc();
                self.base_mut().add_child(&node);
            }
            self.base_mut()
                .set_slot_enabled_right(i.try_into().unwrap(), true);
        }
    }

    fn update_pattern(&mut self) {
        self.brief = ParserNodeBrief {
            pattern: self.parser.as_ref().unwrap().pattern.to_string(),
            ..self.brief
        };
        self.update_brief();
    }

    fn update_brief(&mut self) {
        // let visible = self.parser.as_ref().unwrap().ref_to_id.is_none();
        // self.completion_label.set_visible(visible);

        self.completion_label
            .set_text(&self.brief.get_completion_string());
    }

    fn add_selector_connection_slots(&mut self) {
        self.base_mut().set_slot_enabled_right(0, true);
    }

    fn add_splitter_connection_slots(&mut self) {
        self.base_mut().set_slot_enabled_right(0, true);
    }

    pub fn connect_children(&mut self, child_nodes: Vec<&Gd<ParserGraphNode>>) {
        // let mut slot_idx = 0;
        let self_name = &self.base().get_name();

        for i in 0..child_nodes.len() {
            let child = child_nodes[i];
            let slot = child.bind().parser.as_ref().unwrap().parent_slot.unwrap();

            // add missing slots
            while self.get_slot_count() <= slot {
                self.create_out_slot();
            }

            self.graph
                .connect_node(self_name, slot, &child.get_name(), 0);
            // slot_idx = mut_slot_idx(slot_idx);
        }
    }

    pub fn update_parser_offset(&mut self) {
        let offset = self.base().get_position_offset();

        let p = self
            .parser
            .as_mut()
            .expect("Tried to update parser offset on None parser");
        p.editor_offset_x = offset.x;
        p.editor_offset_y = offset.y;
    }
}
