use godot::classes::tab_bar::CloseButtonDisplayPolicy;
use godot::classes::*;
use godot::prelude::*;

use crate::model::parser::ParserModel;
use crate::model::project::ProjectModel;
use crate::nodes::parser_editor::ParserEditorWindowNode;
use crate::nodes::parser_editor::ParserEditorWindowNodeMode;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::project_tabs::cards::cards_tab::CardsTabNode;
use crate::nodes::project_tabs::logs::logs_tab::LogsTabNode;
use crate::nodes::project_tabs::parsers::parser_tab::ParserTabNode;
use crate::repo::ParserRepositoryNode;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ParsersTabNode {
    base: Base<Control>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,

    #[init(val = OnReady::manual())]
    pub logs_tab: OnReady<Gd<LogsTabNode>>,

    pub loaded_project_name: String,
    pub loaded_templates: Vec<ParserModel>,

    #[export]
    parser_tab_scene: OnEditor<Gd<PackedScene>>,

    #[export]
    cards_tab: OnEditor<Gd<CardsTabNode>>,

    #[export_group(name = "Nodes")]
    #[export]
    templates_list: OnEditor<Gd<ItemList>>,
    #[export]
    parser_tabs_container: OnEditor<Gd<TabContainer>>,
    #[export]
    pub parser_editor_window: OnEditor<Gd<ParserEditorWindowNode>>,
    #[export]
    create_button: OnEditor<Gd<Button>>,
    #[export]
    delete_button: OnEditor<Gd<Button>>,
    #[export]
    template_filter: OnEditor<Gd<LineEdit>>,
    #[export]
    clear_template_filter_button: OnEditor<Gd<Button>>,
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
        self.template_filter
            .signals()
            .text_changed()
            .connect_other(self, Self::on_template_filter_text_changed);
        self.clear_template_filter_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_clear_template_filter_button_pressed);
    }

    fn display_templates_list(&mut self) {
        self.templates_list.clear();
        let filter = self.template_filter.get_text().to_string().to_lowercase();
        for parser in &self.loaded_templates {
            if filter.len() == 0 || parser.name.to_lowercase().contains(&filter) {
                let idx = self.templates_list.add_item(&parser.name);
                // let idx = self.templates_list.add_item(
                //     &format!(
                //         "{}{}",
                //         match project.root_parser_id {
                //             Some(root_id) if root_id == parser.id => "* ",
                //             _ => "",
                //         },
                //         &parser.name
                //     )
                //     .to_string(),
                // );
                self.templates_list
                    .set_item_metadata(idx, &parser.id.to_variant());
            }
            }
    }

    fn on_clear_template_filter_button_pressed(&mut self) {
        self.template_filter.call_deferred("clear", &[]);
    }

    fn on_template_filter_text_changed(&mut self, new_text: GString) {
        self.display_templates_list();
    }

    pub fn set_repo(&mut self, repo: Gd<ParserRepositoryNode>) {
        self.repo.init(repo.clone());

        self.parser_editor_window.bind_mut().repo.init(repo.clone());
    }

    fn on_parser_editor_window_save_request(&mut self) {
        if self.parser_editor_window.bind_mut().mode.as_ref().unwrap()
            == &ParserEditorWindowNodeMode::CreateLocal
        {
            return;
        }
        let model = self.parser_editor_window.bind_mut().build();
        self.parser_editor_window.hide();

        self.repo
            .bind_mut()
            .insert_or_update_parser(&model)
            .expect("Failed to insert or update parser");

        for i in 0..self.parser_tabs_container.get_child_count() {
            let mut child = self
                .parser_tabs_container
                .get_child(i)
                .expect("Failed to get child while iterating over get_children")
                .try_cast::<ParserTabNode>()
                .unwrap();

            // update name
            let actual = self
                .repo
                .bind_mut()
                .get_parser(*child.bind().loaded_id.as_ref().unwrap())
                .expect("Failed to get parser")
                .unwrap();
            child.set_name(&actual.name);

            child.bind_mut().load_parser(&actual);
        }
        self.reload_templates();
    }

    fn on_parser_editor_window_cancel_request(&mut self) {
        self.parser_editor_window.hide();
    }

    fn on_create_button_pressed(&mut self) {
        self.parser_editor_window.bind_mut().mode =
            Some(crate::nodes::parser_editor::ParserEditorWindowNodeMode::CreateTemplate);
        self.parser_editor_window
            .bind_mut()
            .clear(self.loaded_project_name.to_string(), true);
        self.parser_editor_window.set_title("Create a new template");
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

    pub fn reload_templates(&mut self) {
        let project_name = self.loaded_project_name.to_string();
        // let project = self
        //     .repo
        //     .bind_mut()
        //     .get_project(&project_name)
        //     .expect("Failed to get project")
        //     .expect("Failed to find project");

        self.loaded_templates = self
            .repo
            .bind_mut()
            .get_templates(&project_name)
            .expect("Failed to load templates");

        let mut logs = self.logs_tab.bind_mut();
        logs.log(format!(
            "Loaded {} template(s)",
            LogsTabNode::format_count(self.loaded_templates.len())
        ));
        drop(logs);

        self.display_templates_list();
    }

    fn on_templates_list_item_activated(&mut self, idx: i64) {
        let parser_id: i32 = self
            .templates_list
            .get_item_metadata(idx.try_into().unwrap())
            .to::<i32>();
        self.open_template(parser_id);
    }

    pub fn open_template(&mut self, parser_id: i32) {
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
                .expect("Failed to get child while iterating over get_children")
                .try_cast::<ParserTabNode>()
                .unwrap();

            if *child.bind().loaded_id.as_ref().unwrap() != parser.id {
                continue;
            }

            self.parser_tabs_container.set_current_tab(i);
            return;
        }

        let mut node = self.parser_tab_scene.instantiate_as::<ParserTabNode>();
        node.set_name(&parser.name);

        self.parser_tabs_container.add_child(&node);
        self.parser_tabs_container.set_current_tab(prev_child_count);

        node.bind_mut().set_repo(self.repo.clone());
        node.bind_mut().parent.init(self.to_gd().clone());
        node.bind_mut().cards_tab.init(self.cards_tab.clone());
        node.bind_mut().load_parser(&parser);
    }
}
