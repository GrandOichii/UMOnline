use std::cell::OnceCell;

use godot::classes::tab_bar::CloseButtonDisplayPolicy;
use godot::classes::*;
use godot::prelude::*;

use crate::model::parser::ParserModel;
use crate::model::parser::pmt_has_pattern;
use crate::model::parser::pmt_to_string;
use crate::model::project::ProjectModel;
use crate::nodes::project_tabs::logs_tab::LogsTabNode;
use crate::repo::ParserRepositoryNode;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParsersTabNode {
    base: Base<Control>,

    pub repo: OnceCell<Gd<ParserRepositoryNode>>,
    pub logs_tab: OnceCell<Gd<LogsTabNode>>,
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

    fn get_repo(&mut self) -> &mut Gd<ParserRepositoryNode> {
        self.repo.get_mut().expect("repo was not initialized!")
    }

    fn get_logs_tab(&mut self) -> &mut Gd<LogsTabNode> {
        self.logs_tab
            .get_mut()
            .expect("logs_tab was not initialized!")
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
        let repo = self.get_repo();

        let templates = repo
            .bind_mut()
            .get_templates(&project_name)
            .expect("Failed to load templates");

        let mut logs = self.get_logs_tab().bind_mut();
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
            .get_repo()
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
            .set(self.repo.get().unwrap().clone())
            .expect("Failed to pass down repo node to parser tab");

        node.bind_mut().load_parser(&parser);
    }
}

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParserTabNode {
    base: Base<Control>,

    pub repo: OnceCell<Gd<ParserRepositoryNode>>,

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
    fn ready(&mut self) {}
}

impl ParserTabNode {
    fn load_parser(&mut self, parser: &ParserModel) {
        // TODO
        self.name_label.set_text(&parser.name);
        self.type_label.set_text(&pmt_to_string(parser.ptype));
        self.pattern_label.set_text(&parser.pattern);
        self.description_label.set_text(&parser.description);
        self.script_display.set_text(&parser.script);

        self.pattern_container
            .set_visible(pmt_has_pattern(parser.ptype));

        self.load_nodes(parser);
    }

    fn get_repo(&mut self) -> &mut Gd<ParserRepositoryNode> {
        self.repo.get_mut().expect("repo was not initialized!")
    }

    fn load_nodes(&mut self, parser: &ParserModel) {
        let parser_with_children = self.get_repo().bind_mut().get_parser_with_children(parser.id)
            .expect("Failed to read parser with children from DB")
            .unwrap();

        self.remove_graph_nodes();
        self.add_nodes_for(&parser_with_children);
        self.graph.arrange_nodes();

        godot_print!("Child count: {}", parser_with_children.children.len());
    }

    fn add_nodes_for(&mut self, parent: &ParserModel) {
        let mut node = self.parser_graph_node_scene.instantiate_as::<ParserGraphNode>();

        self.graph.add_child(&node);
        node.bind_mut().load_parser(parent);

        godot_print!("LOAD CHILDREN {}", parent.children.len());
        for child in &parent.children {
            self.add_nodes_for(child);
        }
    }

    fn remove_graph_nodes(&mut self) {
        while self.graph.get_child_count() > 1
            && let Some(node) = self.graph.get_child(1)
        {
            self.graph.remove_child(&node);
        }
    }
}


#[derive(GodotClass)]
#[class(init,base=GraphNode)]
pub struct ParserGraphNode {
    base: Base<GraphNode>,
}

#[godot_api]
impl IGraphNode for ParserGraphNode {
    fn ready(&mut self) {}
}

impl ParserGraphNode {
    fn load_parser(&mut self, parser: &ParserModel) {
        godot_print!("LOAD {}", parser.id);
        self.base_mut().set_title(&parser.name);
        // TODO
    }
}