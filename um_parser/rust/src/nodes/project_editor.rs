use std::cell::OnceCell;
use std::cell::RefCell;
use std::collections::HashMap;
use std::error::Error;
use std::rc::Rc;

use godot::classes::*;
use godot::prelude::*;
use mlua::Lua;

use crate::model::parser::ParserModel;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::parsing_history::ParsingHistoryNode;
use crate::nodes::project_tabs::cards_tab::*;
use crate::nodes::project_tabs::logs_tab::LogsTabNode;
use crate::nodes::project_tabs::parsers_tab::ParsersTabNode;
use crate::parsers::parser::ParseResult;
use crate::parsers::parser::ParseResultStatus;
use crate::parsers::parser::ParserNode;
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
        self.parsers_tab.bind_mut().repo.init(self.repo.clone());
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
    fn parse(&mut self) {
        let mut logs_tab = self.get_logs_tab().expect("Failed to get logs tab");

        logs_tab
            .bind_mut()
            .log(String::from("Starting parsing process..."));

        let models = self
            .get_repo()
            .expect("Failed to get repo")
            .bind_mut()
            .get_parsers(&self.edited_project_name)
            .expect("Failed to load templates");

        // Create all parsers
        let mut parsers = Vec::<Rc<RefCell<ParserNode>>>::new();
        let root_idx = OnceCell::<usize>::new();
        for (idx, model) in models.iter().enumerate() {
            if model.is_root {
                root_idx.set(idx).expect("Failed to set root_idx");
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

        // Connect children
        for model in models.iter() {
            if let Some(parent_id) = model.parent_id {
                let parent = parser_id_to_parser[&parent_id].clone();
                let child = parser_id_to_parser[&model.id].clone();
                parent.borrow_mut().children.push(child);
            }
        }

        let root = parsers[*root_idx.get().expect("Failed to find root")].clone();

        let cards = self
            .get_repo()
            .expect("Failed to get repo")
            .bind_mut()
            .get_cards(&self.edited_project_name)
            .expect("Failed to get cards");
        let mut total = 0;
        let mut parsed = 0;

        let mut parse_results = Vec::<ParseResult>::new();

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
                }
                _other => {
                    logs_tab.bind_mut().log(format!(
                        "Failed to parse card {}",
                        LogsTabNode::format_failed_to_parse_card_name(&card.name)
                    ));
                }
            }

            parse_results.push(result);
        }

        logs_tab.bind_mut().log_important(format!(
            "Finished parsing, parsed {}/{} cards",
            LogsTabNode::format_count(parsed),
            LogsTabNode::format_count(total)
        ));

        self.on_new_history_added(ParsingHistory::from_parse_results(parse_results));
    }

    fn connect_signals(&self) {
        self.project_description_edit
            .signals()
            .text_changed()
            .connect_other(self, Self::on_project_description_edit_text_changed);
    }

    pub fn on_new_history_added(&mut self, ph: ParsingHistory) {
        self.update_parsing_progress_bar(&ph);
        // TODO update cards_tab
        self.parsers_tab.bind_mut().update_parsing_history(&ph);

        self.repo.bind_mut().set_current_parsing_history(self.edited_project_name.to_string(), ph);
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
}
