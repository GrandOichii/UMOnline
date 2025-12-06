use godot::classes::tab_bar::CloseButtonDisplayPolicy;
use godot::classes::*;
use godot::prelude::*;
use regex::Regex;

use crate::model::parser::*;
use crate::model::project::ProjectModel;
use crate::nodes::parser_editor::ParserEditorWindowNode;
use crate::nodes::parsing_history::ParserParsingHistory;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::project_tabs::logs_tab::LogsTabNode;
use crate::repo::ParserRepositoryNode;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParsersTabNode {
    base: Base<Control>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,

    #[init(val = OnReady::manual())]
    pub logs_tab: OnReady<Gd<LogsTabNode>>,

    loaded_project_name: String,

    #[export]
    parser_tab_scene: OnEditor<Gd<PackedScene>>,

    #[export_group(name = "Nodes")]
    #[export]
    templates_list: OnEditor<Gd<ItemList>>,
    #[export]
    parser_tabs_container: OnEditor<Gd<TabContainer>>,
    #[export]
    parser_editor_window: OnEditor<Gd<ParserEditorWindowNode>>,
    #[export]
    create_button: OnEditor<Gd<Button>>,
    #[export]
    delete_button: OnEditor<Gd<Button>>,
}

#[godot_api]
impl IControl for ParsersTabNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.parser_editor_window.hide();
        self.templates_list.clear();
        self.parser_tabs_container
            .get_tab_bar()
            .unwrap()
            .set_tab_close_display_policy(CloseButtonDisplayPolicy::SHOW_ALWAYS);

        self.close_parser_tabs();
    }
}

impl ParsersTabNode {
    fn connect_signals(&mut self) {
        self.templates_list
            .signals()
            .item_activated()
            .connect_other(self, Self::on_templates_list_item_activated);
        self.parser_tabs_container
            .get_tab_bar()
            .unwrap()
            .signals()
            .tab_close_pressed()
            .connect_other(self, Self::on_parser_tabs_container_close_pressed);
        self.create_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_create_button_pressed);
        self.parser_editor_window
            .signals()
            .save_request()
            .connect_other(self, Self::on_parser_editor_window_save_request);
        self.parser_editor_window
            .signals()
            .cancel_request()
            .connect_other(self, Self::on_parser_editor_window_cancel_request);
    }

    pub fn set_repo(&mut self, repo: Gd<ParserRepositoryNode>) {
        self.repo.init(repo.clone());

        self.parser_editor_window
            .bind_mut()
            .repo
            .init(repo.clone());
    }

    fn on_parser_editor_window_save_request(&mut self) {
        let model = self.parser_editor_window.bind_mut().build();
        self.parser_editor_window.hide();

        // TODO
        godot_print!("ID: {}", model.id);
        
    }

    fn on_parser_editor_window_cancel_request(&mut self) {
        self.parser_editor_window.hide();
    }

    fn on_create_button_pressed(&mut self) {
        self.parser_editor_window.bind_mut().clear(self.loaded_project_name.to_string());
        self.parser_editor_window.set_title("Create a new parser");
        self.parser_editor_window.show();
    }

    pub fn update_parsing_history(&mut self, ph: &ParsingHistory) {
        for i in 0..self.parser_tabs_container.get_child_count() {
            let mut child = self
                .parser_tabs_container
                .get_child(i)
                .unwrap()
                .try_cast::<ParserTabNode>()
                .expect("Non-ParserTabNode detected in parser_tabs_container");
            child.bind_mut().update_parsing_history(Some(ph));
        }
    }

    fn on_parser_tabs_container_close_pressed(&mut self, idx: i64) {
        let child = self
            .parser_tabs_container
            .get_child(idx.try_into().unwrap())
            .expect("Tried to close a non-existant parser tab");
        self.parser_tabs_container.remove_child(&child);
    }

    fn close_parser_tabs(&mut self) {
        while self.parser_tabs_container.get_child_count() > 0
            && let Some(node) = self.parser_tabs_container.get_child(0)
        {
            self.parser_tabs_container.remove_child(&node);
        }
    }

    pub fn load_project(&mut self, project: &ProjectModel) {
        self.loaded_project_name = project.name.to_string();
        self.close_parser_tabs();
        self.reload_templates();
    }

    fn reload_templates(&mut self) {
        self.templates_list.clear();
        let project_name = self.loaded_project_name.to_string();

        let templates = self
            .repo
            .bind_mut()
            .get_templates(&project_name)
            .expect("Failed to load templates");

        let mut logs = self.logs_tab.bind_mut();
        logs.log(format!(
            "Loaded {} template(s)",
            LogsTabNode::format_count(templates.len())
        ));
        drop(logs);

        self.templates_list.clear();
        for parser in &templates {
            // TODO filter

            let idx = self.templates_list.add_item(
                &format!(
                    "{}{}",
                    match parser.is_root {
                        true => "* ",
                        false => "",
                    },
                    &parser.name
                )
                .to_string(),
            );
            self.templates_list
                .set_item_metadata(idx, &parser.id.to_variant());
        }
    }

    fn on_templates_list_item_activated(&mut self, idx: i64) {
        let parser_id: i32 = self
            .templates_list
            .get_item_metadata(idx.try_into().unwrap())
            .to::<i32>();
        self.open_template(parser_id);
    }

    fn open_template(&mut self, parser_id: i32) {
        let prev_child_count = self.parser_tabs_container.get_child_count();

        let parser = self
            .repo
            .bind_mut()
            .get_parser(parser_id)
            .expect("Failed to load parser")
            .expect("Tried to open a parser tab with a parser that doesnt exist");

        for i in 0..=(prev_child_count - 1) {
            let child = self
                .parser_tabs_container
                .get_child(i)
                .expect("Failed to get child while iterating over get_children");
            if child.get_name().to_string() != parser.name {
                continue;
            }

            self.parser_tabs_container.set_current_tab(i);
            return;
        }

        let mut node = self.parser_tab_scene.instantiate_as::<ParserTabNode>();
        node.set_name(&parser.name);

        self.parser_tabs_container.add_child(&node);
        self.parser_tabs_container.set_current_tab(prev_child_count);

        node.bind_mut().repo.init(self.repo.clone());
        node.bind_mut().parser_editor_window.init(self.parser_editor_window.clone());
        node.bind_mut().load_parser(&parser);
    }
}

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParserTabNode {
    base: Base<Control>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,
    #[init(val = OnReady::manual())]
    parser_editor_window: OnReady<Gd<ParserEditorWindowNode>>,

    #[export]
    parser_graph_node_scene: OnEditor<Gd<PackedScene>>,

    #[export_group(name = "Nodes")]
    #[export]
    name_label: OnEditor<Gd<Label>>,
    #[export]
    type_label: OnEditor<Gd<Label>>,
    #[export]
    pattern_label: OnEditor<Gd<Label>>,
    #[export]
    description_label: OnEditor<Gd<Label>>,
    #[export]
    script_display: OnEditor<Gd<CodeEdit>>,
    #[export]
    pattern_container: OnEditor<Gd<Container>>,
    #[export]
    graph: OnEditor<Gd<GraphEdit>>,
}

#[godot_api]
impl IControl for ParserTabNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl ParserTabNode {
    fn connect_signals(&mut self) {
        self.graph
            .signals()
            .node_selected()
            .connect_other(self, Self::on_graph_node_selected);
        self.graph
            .signals()
            .end_node_move()
            .connect_other(self, Self::on_graph_end_node_move);
        // self.graph
        //     .signals()
        // .node_ac()
        // .connect_other(self, Self::signal_connection);
    }

    pub fn update_parsing_history(&mut self, ph: Option<&ParsingHistory>) {
        for i in 0..self.graph.get_child_count() {
            // let mut child = self.graph.get_child(i).unwrap().try_cast::<ParserTabNode>()
            //     .expect("Non-ParserTabNode detected in parser_tabs_container");
            let child = self.graph.get_child(i).unwrap();
            match child.try_cast::<ParserGraphNode>() {
                Ok(mut node) => {
                    node.bind_mut().load_parsing_history(ph);
                }
                Err(_) => continue,
            };
            // child.bind_mut().update_parsing_history(ph);
        }
    }

    fn get_displayed_parsers(&mut self) -> Vec<Gd<ParserGraphNode>> {
        let child_count = self.graph.get_child_count();

        let mut result = Vec::new();

        for i in 0..child_count {
            let child = self
                .graph
                .get_child(i)
                .expect("Failed to get child at expected idx");
            if let Ok(parser_node) = child.try_cast::<ParserGraphNode>() {
                result.push(parser_node);
            }
        }

        result
    }

    fn on_graph_end_node_move(&mut self) {
        let parsers = self.get_displayed_parsers();
        for mut child in parsers {
            let mut parser = child.bind_mut();
            parser.update_parser_offset();
            self.repo
                .bind_mut()
                .update_parser_by_id(
                    parser
                        .parser
                        .as_ref()
                        .expect("Tried to update a parser that is None"),
                )
                .expect("Failed to update parser");
        }
    }

    fn on_graph_node_selected(&mut self, node: Gd<Node>) {
        godot_print!("NODE SELECTED");
    }

    fn load_parser(&mut self, parser: &ParserModel) {
        self.name_label.set_text(&parser.name);
        self.type_label.set_text(&pmt_to_string(parser.ptype));
        self.pattern_label.set_text(&parser.pattern);
        self.description_label.set_text(&parser.description);
        self.script_display.set_text(&parser.script);

        self.pattern_container
            .set_visible(pmt_has_pattern(parser.ptype));

        self.load_nodes(parser);
    }

    fn load_nodes(&mut self, parser: &ParserModel) {
        let parser_with_children = self
            .repo
            .bind_mut()
            .get_parser_with_children(parser.id)
            .expect("Failed to read parser with children from DB")
            .unwrap();

        self.remove_graph_nodes();
        self.add_nodes_for(&parser_with_children);

        // let mut timer = Timer::new_alloc();
        // self.base_mut().add_child(&timer);
        // timer.set_wait_time(0.1);
        // timer.set_one_shot(true);
        // timer.connect("timeout", &self.graph.callable("arrange_nodes"));
        // timer.start();

        // self.graph.call_deferred("arrange_nodes", &[]);
    }

    fn add_nodes_for(&mut self, parent: &ParserModel) -> Gd<ParserGraphNode> {
        godot_print!("ADD NODES FOR: {} ({})", &parent.name, parent.children.len());
        let mut result = self
            .parser_graph_node_scene
            .instantiate_as::<ParserGraphNode>();
        self.graph.add_child(&result);
        result.bind_mut().graph.init(self.graph.clone());
        result
            .bind_mut()
            .parser_editor_window
            .init(self.parser_editor_window.clone());

        let binding = self.repo.bind_mut();
        let ph = binding.get_parsing_history(&parent.project_name);
        // let ph = binding.get_parser_parsing_history(&parent.project_name, &parent.name);

        result.bind_mut().load_parser(parent.clone());
        result.bind_mut().load_parsing_history(ph);
        drop(binding);

        let mut children = Vec::<Gd<ParserGraphNode>>::with_capacity(parent.children.len());

        for child in &parent.children {
            children.push(self.add_nodes_for(child));
        }
        result.bind_mut().connect_children(parent, children);

        return result;
    }

    fn remove_graph_nodes(&mut self) {
        while self.graph.get_child_count() > 1
            && let Some(node) = self.graph.get_child(1)
        {
            self.graph.remove_child(&node);
        }
    }
}

struct ParserNodeTitle {
    pub pattern: String,
    pub parsed_count: usize,
    pub unparsed_count: usize,
}

impl ParserNodeTitle {
    pub fn to_string(&self) -> String {
        format!(
            "{}/{} {}",
            &self.parsed_count, &self.unparsed_count, &self.pattern
        )
        // format!("{}/{}", &self.parsed_count, &self.unparsed_count)
    }
}

impl Default for ParserNodeTitle {
    fn default() -> Self {
        Self {
            pattern: String::from(""),
            parsed_count: 0,
            unparsed_count: 0,
        }
        // Self { parsed_count: 0, unparsed_count: 0 }
    }
}

#[derive(GodotClass)]
#[class(init,base=GraphNode)]
pub struct ParserGraphNode {
    base: Base<GraphNode>,

    title: ParserNodeTitle,
    parser: Option<ParserModel>,

    #[init(val = OnReady::manual())]
    pub graph: OnReady<Gd<GraphEdit>>,
    #[init(val = OnReady::manual())]
    pub parser_editor_window: OnReady<Gd<ParserEditorWindowNode>>,

    #[export]
    template_color: Color,
    #[export]
    local_color: Color,
    #[export]
    ref_color: Color,

    #[export_group(name = "Nodes")]
    #[export]
    parsing_info_container: OnEditor<Gd<FoldableContainer>>,
    #[export]
    unparsed_texts_list: OnEditor<Gd<ItemList>>,
    #[export]
    parsed_texts_list: OnEditor<Gd<ItemList>>,
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
            if self.parser.as_ref().unwrap().ref_to_id.is_some() {
                return;
            }
            self.parser_editor_window
                .bind_mut()
                .load(&self.parser.as_ref().unwrap());
            self.parser_editor_window.set_title(
                &format!("Edit parser {}", &self.parser.as_ref().unwrap().name).to_string(),
            );
            self.parser_editor_window.show();
            return;
        }
    }

    fn load_parser(&mut self, parser: ParserModel) {
        self.base_mut().set_title(&parser.name);

        // set color
        self.set_color(&parser);
        self.add_connection_slots(&parser);
        self.set_pattern(&parser.pattern);

        self.base_mut()
            .set_position_offset(Vector2::new(parser.editor_offset_x, parser.editor_offset_y));

        self.parser = Some(parser);
    }

    fn load_parsing_history(&mut self, ph: Option<&ParsingHistory>) {
        let binding = ParserParsingHistory {
            parsed_texts: vec![],
            unparsed_texts: vec![],
        };
        let name = &self.parser.as_ref().unwrap().name;
        let v = match ph {
            Some(p) => p.get_for(name).unwrap_or(&binding),
            None => &binding,
        };

        self.title.parsed_count = v.parsed_texts.len();
        self.title.unparsed_count = v.unparsed_texts.len();
        self.update_title();

        // unparsed texts
        self.unparsed_texts_list.clear();
        // for text in &v.unparsed_texts {
        //     self.unparsed_texts_list.add_item(text);
        //     // TODO set metadata
        // }
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
        // let node = Label::new_alloc();
        // self.base_mut().add_child(&node);

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

    fn set_pattern(&mut self, new_pattern: &String) {
        self.title = ParserNodeTitle {
            pattern: new_pattern.to_string(),
            ..self.title
        };
        self.update_title();
    }

    fn update_title(&mut self) {
        self.parsing_info_container
            .set_title(&self.title.to_string());
    }

    fn add_selector_connection_slots(&mut self) {
        self.base_mut().set_slot_enabled_right(0, true);
    }

    fn add_splitter_connection_slots(&mut self) {
        self.base_mut().set_slot_enabled_right(0, true);
    }

    fn connect_children(&mut self, parser: &ParserModel, child_nodes: Vec<Gd<ParserGraphNode>>) {
        let mut slot_idx = 0;
        let mut_slot_idx: fn(i32) -> i32 = match parser.ptype {
            PMT_MATCHER => |idx| idx + 1,
            PMT_SELECTOR => |idx| idx,
            PMT_SPLITTER => |idx| idx,
            other => panic!("Unrecognized parser model type: {}", other),
        };
        let self_name = &self.base().get_name();

        // godot_print!("CONNECTING");
        for i in 0..child_nodes.len() {
            self.graph
                .connect_node(self_name, slot_idx, &child_nodes[i].get_name(), 0);
            slot_idx = mut_slot_idx(slot_idx);
        }
    }

    fn update_parser_offset(&mut self) {
        let offset = self.base().get_position_offset();

        let p = self
            .parser
            .as_mut()
            .expect("Tried to update parser offset on None parser");
        p.editor_offset_x = offset.x;
        p.editor_offset_y = offset.y;
    }
}
