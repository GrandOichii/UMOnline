use std::collections::HashMap;

use godot::classes::*;
use godot::prelude::*;

use crate::model::parser::*;
use crate::nodes::parser_editor::ParserEditorWindowNode;
use crate::nodes::parser_editor::ParserEditorWindowNodeMode;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::project_tabs::parsers::parsed_text::ParsedTextNode;
use crate::nodes::project_tabs::parsers::parser_graph_node::ParserGraphNode;
use crate::nodes::project_tabs::parsers::parsers_tab::ParsersTabNode;
use crate::repo::ParserRepositoryNode;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParserTabNode {
    base: Base<Control>,

    pub loaded_id: Option<i32>,
    pub last_popup_position: Option<Vector2>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,
    #[init(val = OnReady::manual())]
    pub parent: OnReady<Gd<ParsersTabNode>>,

    #[export]
    parser_graph_node_scene: OnEditor<Gd<PackedScene>>,
    #[export]
    parsed_text_scene: OnEditor<Gd<PackedScene>>,

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
    #[export]
    new_node_menu: OnEditor<Gd<PopupMenu>>,
    #[export]
    matchers_popup_submenu: OnEditor<Gd<PopupMenu>>,
    #[export]
    selectors_popup_submenu: OnEditor<Gd<PopupMenu>>,
    #[export]
    splitters_popup_submenu: OnEditor<Gd<PopupMenu>>,
    #[export]
    parsed_container: OnEditor<Gd<Container>>,
}

#[godot_api]
impl IControl for ParserTabNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.configure_new_node_menu();
    }
}

impl ParserTabNode {
    fn connect_signals(&mut self) {
        // self.graph
        //     .signals()
        //     .node_selected()
        //     .connect_other(self, Self::on_graph_node_selected);
        self.graph
            .signals()
            .end_node_move()
            .connect_other(self, Self::on_graph_end_node_move);
        self.graph
            .signals()
            .popup_request()
            .connect_other(self, Self::on_graph_popup_request);
        self.graph
            .signals()
            .connection_request()
            .connect_other(self, Self::on_graph_connection_request);
        self.graph
            .signals()
            .disconnection_request()
            .connect_other(self, Self::on_graph_disconnection_request);
        self.new_node_menu
            .signals()
            .index_pressed()
            .connect_other(self, Self::on_new_node_menu_index_pressed);
        self.matchers_popup_submenu
            .signals()
            .index_pressed()
            .connect_other(self, Self::on_matchers_popup_submenu_index_pressed);
        self.selectors_popup_submenu
            .signals()
            .index_pressed()
            .connect_other(self, Self::on_selectors_popup_submenu_index_pressed);
        self.splitters_popup_submenu
            .signals()
            .index_pressed()
            .connect_other(self, Self::on_splitters_popup_submenu_index_pressed);
        self.graph
            .signals()
            .delete_nodes_request()
            .connect_other(self, Self::on_graph_delete_nodes_request);
    }

    pub fn connect_editor_window_signals(&mut self, editor_window: Gd<ParserEditorWindowNode>) {
        editor_window
            .signals()
            .save_request()
            .connect_other(self, Self::on_parser_editor_window_save_request);
    }

    fn on_parser_editor_window_save_request(&mut self) {
        let mut editor_window = self.parent.bind_mut().parser_editor_window.clone();
        if editor_window.bind_mut().mode.as_ref().unwrap()
            != &ParserEditorWindowNodeMode::CreateLocal
        {
            return;
        }
        let model = editor_window.bind_mut().build();
        editor_window.hide();

        self.create_node(model);
    }

    fn create_node(&mut self, mut parser: ParserModel) {
        parser.parser_editor_id = self.loaded_id.unwrap();
        parser.editor_offset_x = (self.last_popup_position.unwrap().x
            + self.graph.get_scroll_offset().x)
            / self.graph.get_zoom();
        parser.editor_offset_y = (self.last_popup_position.unwrap().y
            + self.graph.get_scroll_offset().y)
            / self.graph.get_zoom();
        self.repo
            .bind_mut()
            .insert_parser(&parser)
            .expect("Failed to create a ref to parser");

        self.load_nodes();
    }

    fn on_graph_delete_nodes_request(&mut self, node_names: Array<StringName>) {
        for node_name in node_names.iter_shared() {
            let node = self
                .graph
                .get_node_as::<ParserGraphNode>(&node_name.to_string());
            let parser = node.bind().parser.as_ref().unwrap().clone();
            if parser.is_template {
                continue;
            }
            if parser.parent_id.is_some() {
                continue;
            }
            self.repo
                .bind_mut()
                .delete_parser(parser.id)
                .expect("Failed to delete parser");
        }
        self.load_nodes();
    }

    fn on_new_node_submenu_index_pressed(&mut self, menu: Gd<PopupMenu>, idx: i32) {
        let parser_id: i32 = menu.get_item_metadata(idx).to();
        let parser = self
            .repo
            .bind_mut()
            .get_parser(parser_id)
            .expect("Failed to get parser")
            .expect("Failed to find pressed parser");

        let parser: ParserModel = ParserModel::new_ref(&parser);
        self.create_node(parser);
        // self.add_nodes_for(&parser);
    }

    fn on_matchers_popup_submenu_index_pressed(&mut self, idx: i64) {
        self.on_new_node_submenu_index_pressed(
            self.matchers_popup_submenu.clone(),
            idx.try_into().unwrap(),
        );
    }

    fn on_selectors_popup_submenu_index_pressed(&mut self, idx: i64) {
        self.on_new_node_submenu_index_pressed(
            self.selectors_popup_submenu.clone(),
            idx.try_into().unwrap(),
        );
    }

    fn on_splitters_popup_submenu_index_pressed(&mut self, idx: i64) {
        self.on_new_node_submenu_index_pressed(
            self.splitters_popup_submenu.clone(),
            idx.try_into().unwrap(),
        );
    }

    fn on_new_node_menu_index_pressed(&mut self, idx: i64) {
        if idx != 0 {
            return;
        }

        let mut editor_window = self.parent.bind_mut().parser_editor_window.clone();

        editor_window.bind_mut().mode =
            Some(crate::nodes::parser_editor::ParserEditorWindowNodeMode::CreateLocal);
        editor_window.bind_mut().clear(
            self.parent.bind_mut().loaded_project_name.to_string(),
            false,
        );
        editor_window.set_title("Create a new local parser");
        editor_window.show();
    }

    fn configure_new_node_menu(&mut self) {
        self.new_node_menu.add_submenu_item(
            "Matchers",
            self.matchers_popup_submenu.get_name().to_string().as_str(),
        );
        self.new_node_menu.add_submenu_item(
            "Selectors",
            self.selectors_popup_submenu.get_name().to_string().as_str(),
        );
        self.new_node_menu.add_submenu_item(
            "Splitters",
            self.splitters_popup_submenu.get_name().to_string().as_str(),
        );
    }

    fn on_graph_popup_request(&mut self, at_position: Vector2) {
        self.last_popup_position = Some(at_position);

        self.matchers_popup_submenu.clear();
        self.selectors_popup_submenu.clear();
        self.splitters_popup_submenu.clear();

        let project_name = self
            .repo
            .bind_mut()
            .get_parser(self.loaded_id.unwrap())
            .expect("Failed to get parser")
            .expect("Failed to find loaded parser")
            .project_name;
        let templates = self
            .repo
            .bind_mut()
            .get_templates(&project_name)
            .expect("Failed to get templates");
        for template in templates {
            let menu = match template.ptype {
                PMT_MATCHER => &mut self.matchers_popup_submenu,
                PMT_SELECTOR => &mut self.selectors_popup_submenu,
                PMT_SPLITTER => &mut self.splitters_popup_submenu,
                other => panic!("Unrecognized parser model type: {}", other),
            };
            let idx = menu.get_item_count();
            menu.add_item(&template.name);
            menu.set_item_metadata(idx, &template.id.to_variant());
        }
        self.new_node_menu.popup();
        self.new_node_menu.set_position(Vector2i::from_tuple((
            (at_position.x + self.graph.get_global_position().x) as i32,
            (at_position.y + self.graph.get_global_position().y) as i32,
        )));
    }

    fn on_graph_connection_request(
        &mut self,
        from: StringName,
        from_slot: i64,
        to: StringName,
        to_slot: i64,
    ) {
        // TODO! check if already is connected
        let parent_node = self.graph.get_node_as::<ParserGraphNode>(&from.to_string());
        let child_node = self.graph.get_node_as::<ParserGraphNode>(&to.to_string());

        let parent_parser = parent_node.bind().parser.as_ref().unwrap().clone();
        let mut child_parser = child_node.bind().parser.as_ref().unwrap().clone();
        child_parser.parent_id = Some(parent_parser.id);
        child_parser.parent_slot = Some(match parent_parser.ptype {
            PMT_MATCHER => from_slot.try_into().unwrap(),
            _ => 0,
        });

        self.repo
            .bind_mut()
            .update_parser_by_id(&child_parser)
            .expect("Failed to save parser with new parent");
        self.load_nodes();
    }

    fn on_graph_disconnection_request(
        &mut self,
        from: StringName,
        from_slot: i64,
        to: StringName,
        to_slot: i64,
    ) {
        // let parent_node = self.graph.get_node_as::<ParserGraphNode>(&from.to_string());
        let child_node = self.graph.get_node_as::<ParserGraphNode>(&to.to_string());

        // let parent_parser = parent_node.bind().parser.as_ref().unwrap().clone();
        let mut child_parser = child_node.bind().parser.as_ref().unwrap().clone();

        child_parser.parent_id = None;
        child_parser.parent_slot = None;
        self.repo
            .bind_mut()
            .update_parser_by_id(&child_parser)
            .expect("Failed to save disconnected parser");
        // self.graph.disconnect_node(
        //     &from.to_string(),
        //     from_slot.try_into().unwrap(),
        //     &to.to_string(),
        //     to_slot.try_into().unwrap(),
        // );
        self.load_nodes();
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

    // fn on_graph_node_selected(&mut self, node: Gd<Node>) {
    //     let mut graph_node = node.try_cast::<ParserGraphNode>().unwrap();
    //     self.load_parser_info(graph_node.bind_mut().parser.as_ref().unwrap())
    // }

    pub fn load_parser_info(&mut self, parser: &ParserModel) {
        let mut p = parser.clone();
        if let Some(ref_id) = p.ref_to_id {
            p = self
                .repo
                .bind_mut()
                .get_parser(ref_id)
                .expect("Failed to get ref parser")
                .expect("Failed to find ref parser");
        }
        self.name_label.set_text(&p.name);
        self.type_label.set_text(&pmt_to_string(p.ptype));
        self.pattern_label.set_text(&p.pattern);
        self.description_label.set_text(&p.description);
        self.script_display.set_text(&p.script);

        self.pattern_container.set_visible(pmt_has_pattern(p.ptype));

        self.load_parsed_texts(&p);
    }

    pub fn load_parser(&mut self, parser: &ParserModel) {
        self.loaded_id = Some(parser.id);

        self.load_parser_info(parser);

        self.load_nodes();
    }

    fn load_nodes(&mut self) {
        self.remove_graph_nodes();

        let nodes = self
            .repo
            .bind_mut()
            .get_editor_parsers(self.loaded_id.unwrap())
            .expect("Failed to get parser editor nodes");

        let mut node_ids: Vec<i32> = vec![];
        let mut node_map = HashMap::<i32, Gd<ParserGraphNode>>::new();
        for node in nodes {
            let mut result = self
                .parser_graph_node_scene
                .instantiate_as::<ParserGraphNode>();
            self.graph.add_child(&result);
            result.bind_mut().graph.init(self.graph.clone());
            result.bind_mut().parent.init(self.to_gd().clone());
            result.bind_mut().parsers_tab.init(self.parent.clone());

            node_ids.push(node.id);
            node_map.insert(node.id, result.clone());

            let binding = self.repo.bind_mut();
            let ph = binding.get_parsing_history(&node.project_name);

            result.bind_mut().load_parser(node);
            result.bind_mut().load_parsing_history(ph);
        }

        // add connections
        for node_id in node_ids {
            let children_ids = self
                .repo
                .bind_mut()
                .get_parser_children_ids(node_id)
                .expect("Failed to get children ids");
            let mut node = node_map.get(&node_id).unwrap().clone();
            let mut children = vec![];
            for child_id in children_ids {
                children.push(node_map.get(&child_id).unwrap());
            }
            node.bind_mut().connect_children(children);
        }
    }

    fn remove_graph_nodes(&mut self) {
        while self.graph.get_child_count() > 1
            && let Some(node) = self.graph.get_child(1)
        {
            self.graph.remove_child(&node);
        }
    }

    fn load_parsed_texts(&mut self, parser: &ParserModel) {
        // remove old entries
        while self.parsed_container.get_child_count() > 0
            && let Some(node) = self.parsed_container.get_child(0)
        {
            self.parsed_container.remove_child(&node);
        }

        // add new entries
        let repo = self.repo.bind_mut();

        let pho = repo.get_parser_parsing_history(&parser.project_name, &parser.name);
        if let Some(ph) = pho {
            for parsed_text in ph.parsed_texts.iter() {
                let mut node = self.parsed_text_scene.instantiate_as::<ParsedTextNode>();
                self.parsed_container.add_child(&node);

                node.bind_mut().load_parsed_text(&parsed_text);
            }
        }
    }
}
