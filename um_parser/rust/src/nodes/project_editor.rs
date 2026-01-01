use std::cell::OnceCell;
use std::cell::RefCell;
use std::collections::HashMap;
use std::rc::Rc;

use godot::classes::*;
use godot::prelude::*;
use mlua::Lua;

use crate::nodes::parsing_history::*;
use crate::nodes::project_tabs::cards::cards_tab::CardsTabNode;
use crate::nodes::project_tabs::logs::logs_tab::LogsTabNode;
use crate::nodes::project_tabs::parsers::parsers_tab::ParsersTabNode;
use crate::parsers::parser::*;
use crate::repo::*;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ProjectEditorNode {
    base: Base<Control>,
    pub edited_project_name: String,

    #[export]
    pub repo: OnEditor<Gd<ParserRepositoryNode>>,
    #[export_group(name = "Nodes")]
    #[export]
    project_name_label: OnEditor<Gd<Label>>,
    #[export]
    project_description_edit: OnEditor<Gd<TextEdit>>,
    #[export]
    cards_tab: OnEditor<Gd<CardsTabNode>>,
    #[export]
    logs_tab: OnEditor<Gd<LogsTabNode>>,
    #[export]
    parsers_tab: OnEditor<Gd<ParsersTabNode>>,
    #[export]
    completion_progress_bar: OnEditor<Gd<ProgressBar>>,
    #[export]
    root_parser_option: OnEditor<Gd<OptionButton>>,
    #[export]
    tabs_node: OnEditor<Gd<TabContainer>>,
    #[export]
    parse_button: OnEditor<Gd<Button>>,
    #[export]
    cannot_parse_dialog: OnEditor<Gd<AcceptDialog>>,
}

#[godot_api]
impl IControl for ProjectEditorNode {
    fn input(&mut self, event: Gd<InputEvent>) {
        if event.is_action_pressed("parse") {
            self.parse();
        }
    }

    fn ready(&mut self) {
        self.connect_signals();

        self.cards_tab.bind_mut().repo.init(self.repo.clone());
        self.cards_tab
            .bind_mut()
            .logs_tab
            .init(self.logs_tab.clone());

        // parsers tab
        self.parsers_tab.bind_mut().set_repo(self.repo.clone());
        self.parsers_tab
            .bind_mut()
            .logs_tab
            .init(self.logs_tab.clone());

        // self.on_new_history_added(ParsingHistory {
        //     parse_results: vec![],
        // });

        self.completion_progress_bar.set_max(1.0);
        self.completion_progress_bar.set_value(0.0);
    }
}

impl ProjectEditorNode {
    fn display_parse_error(&mut self, err_msg: &str) {
        self.cannot_parse_dialog.set_text(err_msg);
        self.cannot_parse_dialog.show();
    }

    fn parse(&mut self) {
        let root_id = self
            .repo
            .bind_mut()
            .get_project(&self.edited_project_name)
            .expect("Failed to get project")
            .expect("Failed to find project")
            .root_parser_id;

        let mut logs_tab = self.get_logs_tab().expect("Failed to get logs tab");

        logs_tab
            .bind_mut()
            .log(String::from("Starting parsing process"));
        logs_tab
            .bind_mut()
            .log(String::from("Building parser tree"));

        let models = self
            .get_repo()
            .expect("Failed to get repo")
            .bind_mut()
            .get_parsers(&self.edited_project_name)
            .expect("Failed to load templates");

        logs_tab.bind_mut().log(String::from("Creating parsers"));
        // Create all parsers
        let mut parsers = Vec::<Rc<RefCell<ParserNode>>>::new();
        let mut root_idx = None;
        for (idx, model) in models.iter().enumerate() {
            if let Some(rid) = root_id
                && rid == model.id
            {
                root_idx = Some(idx);
            }
            parsers.push(Rc::new(RefCell::new(
                model
                    .to_parser_node()
                    .expect("Failed to create parser node"),
            )));
        }

        // Create parser_id_to_parser
        let mut parser_id_to_parser = HashMap::<i32, Rc<RefCell<ParserNode>>>::new();
        for (idx, parser) in models.iter().enumerate() {
            parser_id_to_parser.insert(parser.id, Rc::clone(&parsers[idx]));
        }

        logs_tab
            .bind_mut()
            .log(String::from("Mapping parser relations"));

        // Create parent-to-children mappings + resolve ref parsers
        let mut parent_to_children = HashMap::<i32, Vec<(i32, i32)>>::new();
        for model in models.iter() {
            if let Some(parent_id) = model.parent_id {
                let list = parent_to_children.entry(parent_id).or_insert(vec![]);
                let mut child_id = model.id;
                if let Some(ref_id) = model.ref_to_id {
                    child_id = ref_id;
                }
                list.push((
                    child_id,
                    model
                        .parent_slot
                        .expect("Found a parser that has a parent_id, but doesnt have parent_slot"),
                ));
            }
        }

        for (_, value) in parent_to_children.iter_mut() {
            value.sort_by(|a, b| a.1.cmp(&b.1));
        }

        logs_tab
            .bind_mut()
            .log(String::from("Connecting child parsers"));

        // Connect children
        for (parent_id, child_ids) in parent_to_children.iter() {
            let parent = parser_id_to_parser[&parent_id].clone();
            for (child_id, _) in child_ids {
                let child = parser_id_to_parser[child_id].clone();
                parent.borrow_mut().children.push(child);
            }
        }

        // TODO notify user that root is not set
        if root_idx.is_none() {
            self.display_parse_error("Root is not set!");
            return;
        }
        // let ridx = match root_idx {
        //     Some(v) => v,
        //     None => {
        //         godot_print!("Root is not set!");
        //         return;
        //     }
        // };
        let root = parsers[*root_idx.as_ref().unwrap()].clone();
        logs_tab.bind_mut().log(String::from(
            "Created parser tree, starting parsing process",
        ));

        let cards = self
            .get_repo()
            .expect("Failed to get repo")
            .bind_mut()
            .get_cards(&self.edited_project_name)
            .expect("Failed to get cards");
        let mut total = 0;
        let mut parsed = 0;

        let mut parse_results = Vec::<(i32, ParseResult)>::new();
        let mut card_scripts = HashMap::<i32, String>::new();

        for card in cards {
            logs_tab
                .bind_mut()
                .log(format!("Parsing card {}", &card.name));
            total += 1;
            let lua = Lua::new();
            let result = ParserNode::parse(root.clone(), &card.text, &lua);
            match result.status {
                ParseResultStatus::Success => {
                    parsed += 1;
                    logs_tab.bind_mut().log(format!(
                        "Parsed card {}",
                        LogsTabNode::format_card_name(&card.name)
                    ));
                    card_scripts.insert(card.id, result.generated.to_string());
                }
                _other => {
                    logs_tab.bind_mut().log(format!(
                        "Failed to parse card {}",
                        LogsTabNode::format_failed_to_parse_card_name(&card.name)
                    ));
                }
            }

            parse_results.push((card.id, result));
        }

        logs_tab.bind_mut().log_important(format!(
            "Finished parsing, parsed {}/{} cards",
            LogsTabNode::format_count(parsed),
            LogsTabNode::format_count(total)
        ));

        self.on_new_history_added(ParsingHistory::new(card_scripts, parse_results));
    }

    fn connect_signals(&self) {
        self.project_description_edit
            .signals()
            .text_changed()
            .connect_other(self, Self::on_project_description_edit_text_changed);
        self.root_parser_option
            .signals()
            .item_selected()
            .connect_other(self, Self::on_root_parser_option_item_selected);
        self.tabs_node
            .signals()
            .tab_changed()
            .connect_other(self, Self::on_tabs_tab_changed);
        self.parse_button
            .signals()
            .pressed()
            .connect_other(self, Self::parse);
    }

    fn on_tabs_tab_changed(&mut self, tab_idx: i64) {
        if tab_idx != 0 {
            return;
        }
        self.update_root_parser_options();
    }

    fn on_root_parser_option_item_selected(&mut self, idx: i64) {
        let new_root_id: i32 = self
            .root_parser_option
            .get_item_metadata(idx.try_into().unwrap())
            .to();
        let new_root = self
            .repo
            .bind_mut()
            .get_parser(new_root_id)
            .expect("Failed to get new_root")
            .expect("Failed to find new_root");
        let mut project = self
            .repo
            .bind_mut()
            .get_project(&self.edited_project_name)
            .expect("Failed to get project")
            .expect("Failed to find project");
        project.root_parser_id = Some(new_root_id);
        self.repo
            .bind_mut()
            .update_project_by_id(&project)
            .expect("Failed to update root_parser_id of project");
        // let old_root = self
        //     .repo
        //     .bind_mut()
        //     .get_root_parser(&self.edited_project_name)
        //     .expect("Failed to get old root");
        // if let Some(mut root) = old_root {
        //     root.is_root = false;
        //     self.repo
        //         .bind_mut()
        //         .update_parser_by_id(&root)
        //         .expect("Failed to update old_root");
        // }

        // let mut new_root = self
        //     .repo
        //     .bind_mut()
        //     .get_parser(new_root_id)
        //     .expect("Failed to get new_root")
        //     .expect("Failed to find new_root");
        // new_root.is_root = true;
        // self.repo
        //     .bind_mut()
        //     .update_parser_by_id(&new_root)
        //     .expect("Failed to update new_root");

        self.logs_tab
            .bind_mut()
            .log(format!("Updated root parser to {}", &new_root.name));
        self.parsers_tab.bind_mut().reload_templates();
    }

    pub fn on_new_history_added(&mut self, ph: ParsingHistory) {
        self.update_parsing_progress_bar(&ph);
        self.parsers_tab.bind_mut().update_parsing_history(&ph);
        self.cards_tab.bind_mut().update_parsing_history(&ph);

        self.repo
            .bind_mut()
            .set_current_parsing_history(self.edited_project_name.to_string(), ph);
        // TODO update displayed parsed and unparsed texts in parsers_tab
    }

    fn update_parsing_progress_bar(&mut self, ph: &ParsingHistory) {
        self.get_completion_progress_bar()
            .expect("Failed to get progress bar")
            .set_max(ph.total_len() as f64);
        self.get_completion_progress_bar()
            .expect("Failed to get progress bar")
            .set_value(ph.parsed_len() as f64);
    }

    pub fn load_project(&mut self, project_name: &String) {
        self.edited_project_name = project_name.to_string();

        let project = self
            .repo
            .bind_mut()
            .get_project(project_name)
            .expect("Failed to read project from DB")
            .expect("Tried to load project which doesn't exist");

        self.project_name_label.set_text(&project.name);
        self.project_description_edit.set_text(&project.description);

        self.logs_tab.bind_mut().load_project(&project);
        self.cards_tab.bind_mut().load_project(&project);
        self.parsers_tab.bind_mut().load_project(&project);

        self.update_root_parser_options();
    }

    fn on_project_description_edit_text_changed(&mut self) {
        let updated_count = self.repo.bind_mut().update_project_description(
            &self.edited_project_name,
            &self.project_description_edit.get_text().to_string(),
        );
        if updated_count != 1 {
            panic!(
                "Expected updated_count to be 1, but it was: {}",
                updated_count
            );
        }
    }

    fn update_root_parser_options(&mut self) {
        let root_id = self
            .repo
            .bind_mut()
            .get_project(&self.edited_project_name)
            .expect("Failed to get project")
            .expect("Failed to find project")
            .root_parser_id;
        let parsers = self
            .repo
            .bind_mut()
            .get_templates(&self.edited_project_name)
            .expect("Failed to get parsers for project");
        self.root_parser_option.clear();

        let mut root_idx: Option<i32> = None;
        for parser in parsers {
            self.root_parser_option.add_item(&parser.name);
            let idx = self.root_parser_option.get_item_count() - 1;
            self.root_parser_option
                .set_item_metadata(idx, &parser.id.to_variant());

            if let Some(rid) = root_id
                && parser.id == rid
            {
                root_idx = Some(idx);
            }
        }
        self.root_parser_option.select(match root_idx {
            Some(idx) => idx,
            None => -1,
        });
    }
}
