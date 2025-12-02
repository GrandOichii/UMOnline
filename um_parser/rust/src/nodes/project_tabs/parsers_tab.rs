use std::cell::Cell;
use std::cell::OnceCell;

use godot::classes::tab_bar::CloseButtonDisplayPolicy;
use godot::classes::*;
use godot::prelude::*;
use regex::Regex;

use crate::model::parser::*;
use crate::model::project::ProjectModel;
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
}

#[godot_api]
impl IControl for ParsersTabNode {
    fn ready(&mut self) {
        self.connect_signals();

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

        let templates = self.repo
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

            let idx = self.templates_list.add_item(&parser.name);
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

        node.bind_mut()
            .repo
            .init(self.repo.clone());

        node.bind_mut().load_parser(&parser);
    }
}

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParserTabNode {
    base: Base<Control>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,

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

        // TODO connect to repo's parser_updated signal, iterate over all displayed parsers, update each one
    }

    fn get_displayed_parsers(&mut self) -> Vec<Gd<ParserGraphNode>> {
        let child_count = self.graph.get_child_count();

        let mut result = Vec::new();

        for i in 0..child_count {
            let child = self.graph.get_child(i)
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
            self.repo.bind_mut().update_parser_by_id(parser.parser.as_ref().expect("Tried to update a parser that is None"))
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
        let mut result = self
            .parser_graph_node_scene
            .instantiate_as::<ParserGraphNode>();
        self.graph.add_child(&result);
        result
            .bind_mut()
            .graph
            .set(self.graph.clone())
            .expect("Failed to pass down graph node");

        result.bind_mut().load_parser(parent.clone());

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
    pub parsed_count: u32,
    pub unparsed_count: u32,
}

impl ParserNodeTitle {
    pub fn to_string(&self) -> String {
        format!("{}/{} {}", &self.parsed_count, &self.unparsed_count, &self.pattern)
        // format!("{}/{}", &self.parsed_count, &self.unparsed_count)
    }
}

impl Default for ParserNodeTitle {
    fn default() -> Self {
        Self { pattern: String::from(""), parsed_count: 0, unparsed_count: 0 }
        // Self { parsed_count: 0, unparsed_count: 0 }
    }
}

#[derive(GodotClass)]
#[class(init,base=GraphNode)]
pub struct ParserGraphNode {
    base: Base<GraphNode>,

    title: ParserNodeTitle,
    parser: Option<ParserModel>,

    pub graph: OnceCell<Gd<GraphEdit>>,

    #[export]
    template_color: Color,
    #[export]
    local_color: Color,
    #[export]
    ref_color: Color,
}

#[godot_api]
impl IGraphNode for ParserGraphNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl ParserGraphNode {
    fn connect_signals(&mut self) {
    }

    fn load_parser(&mut self, parser: ParserModel) {
        self.base_mut().set_title(&parser.name);

        // set color
        self.set_color(&parser);
        self.add_connection_slots(&parser);
        self.set_pattern(&parser.pattern);

        self.base_mut().set_position_offset(Vector2::new(parser.editor_offset_x, parser.editor_offset_y));

        self.parser = Some(parser);
    }

    fn set_self_color(&mut self, color: Color) {
        self.base_mut().set_self_modulate(color);
    }

    fn set_color(&mut self, parser: &ParserModel) {
        if parser.is_ref {
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

        if parser.is_ref {
            return;
        }

        match parser.ptype {
            PMT_MATCHER => self.add_matcher_connection_slots(parser),
            PMT_SELECTOR => self.add_selector_connection_slots(parser),
            PMT_SPLITTER => self.add_splitter_connection_slots(parser),
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
        let mut display: Gd<FoldableContainer> = self.base().get_child(0).expect("msg").try_cast().unwrap();
        display.set_title(&self.title.to_string());
    }

    fn add_selector_connection_slots(&mut self, parser: &ParserModel) {
        self.base_mut().set_slot_enabled_right(0, true);
    }

    fn add_splitter_connection_slots(&mut self, parser: &ParserModel) {
        self.base_mut().set_slot_enabled_right(0, true);
    }

    fn get_graph(&mut self) -> &mut Gd<GraphEdit> {
        self.graph.get_mut().expect("graph was not initialized!")
    }

    fn connect_children(&mut self, parser: &ParserModel, child_nodes: Vec<Gd<ParserGraphNode>>) {
        let mut slot_idx = 0;
        let mut_slot_idx: fn(i32) -> i32 = match parser.ptype {
            PMT_MATCHER => |idx| idx + 1,
            PMT_SELECTOR => |idx| idx,
            PMT_SPLITTER => |idx| idx,
            // PMT_SPLITTER => |_| panic!("Tried to connect multiple children to splitter"),
            other => panic!("Unrecognized parser model type: {}", other),
        };
        let self_name = &self.base().get_name();

        // godot_print!("CONNECTING");
        for i in 0..child_nodes.len() {
            // TODO connect
            // godot_print!("CONNECT {} WITH SLOT_IDX {}", &parser.name, slot_idx);
            self.get_graph()
                .connect_node_ex(self_name, slot_idx, &child_nodes[i].get_name(), 0)
                .keep_alive(true)
                .done();
            slot_idx = mut_slot_idx(slot_idx);
        }
    }

    fn update_parser_offset(&mut self) {
        let offset = self.base().get_position_offset();

        let p = self.parser.as_mut().expect("Tried to update parser offset on None parser");
        p.editor_offset_x = offset.x;
        p.editor_offset_y = offset.y;
    }
}
