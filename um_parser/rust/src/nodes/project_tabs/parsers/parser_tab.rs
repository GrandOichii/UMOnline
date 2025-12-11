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
use crate::nodes::project_tabs::parsers::parser_graph_node::ParserGraphNode;
use crate::nodes::project_tabs::parsers::parsers_tab::ParsersTabNode;
use crate::repo::ParserRepositoryNode;


#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParserTabNode {
    base: Base<Control>,

    pub loaded_id: Option<i32>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,
    #[init(val = OnReady::manual())]
    pub parent: OnReady<Gd<ParsersTabNode>>,

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
        //     .connection_request()
        //     .connect_other(self, Self::on_graph_connection_request);
        // self.graph
        //     .signals()
        //     .disconnection_request()
        //     .connect_other(self, Self::on_graph_disconnection_request);
    }

    fn on_graph_connection_request(
        &mut self,
        from: StringName,
        from_slot: i64,
        to: StringName,
        to_slot: i64,
    ) {
        // TODO
        godot_print!("CONNECTION REQUEST");
    }

    fn on_graph_disconnection_request(
        &mut self,
        from: StringName,
        from_slot: i64,
        to: StringName,
        to_slot: i64,
    ) {
        // TODO
        godot_print!("DISCONNECTION REQUEST");
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
        let mut graph_node = node.try_cast::<ParserGraphNode>().unwrap();
        self.load_parser_info(graph_node.bind_mut().parser.as_ref().unwrap())
    }

    pub fn load_parser_info(&mut self, parser: &ParserModel) {
        // TODO check if is reference
        self.loaded_id = Some(parser.id);
        self.name_label.set_text(&parser.name);
        self.type_label.set_text(&pmt_to_string(parser.ptype));
        self.pattern_label.set_text(&parser.pattern);
        self.description_label.set_text(&parser.description);
        self.script_display.set_text(&parser.script);

        self.pattern_container
            .set_visible(pmt_has_pattern(parser.ptype));
    }

    pub fn load_parser(&mut self, parser: &ParserModel) {
        self.load_parser_info(parser);

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
    }

    fn add_nodes_for(&mut self, parent: &ParserModel) -> Gd<ParserGraphNode> {
        let mut result = self
            .parser_graph_node_scene
            .instantiate_as::<ParserGraphNode>();
        self.graph.add_child(&result);
        result.bind_mut().graph.init(self.graph.clone());
        result.bind_mut().parent.init(self.parent.clone());

        let binding = self.repo.bind_mut();
        let ph = binding.get_parsing_history(&parent.project_name);

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
