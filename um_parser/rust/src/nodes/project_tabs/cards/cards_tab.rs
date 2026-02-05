use std::fs;

use godot::classes::tab_bar::CloseButtonDisplayPolicy;
use godot::classes::*;
use godot::prelude::*;
use serde::Deserialize;

use crate::model::card::CardModel;
use crate::model::project::ProjectModel;
use crate::nodes::parsing_history::ParsingHistory;
use crate::nodes::project_tabs::cards::card_tab::CardTabNode;
use crate::nodes::project_tabs::cards::cards_manager::CardsManagerWindowNode;
use crate::nodes::project_tabs::logs::logs_tab::LogsTabNode;
use crate::repo::*;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardsTabNode {
    base: Base<Control>,

    #[init(val = OnReady::manual())]
    pub repo: OnReady<Gd<ParserRepositoryNode>>,

    #[init(val = OnReady::manual())]
    pub logs_tab: OnReady<Gd<LogsTabNode>>,

    loaded_project_name: String,

    #[export]
    card_tab_scene: OnEditor<Gd<PackedScene>>,

    #[export_group(name = "Nodes")]
    #[export]
    import_cards_button: OnEditor<Gd<Button>>,
    #[export]
    cards_list: OnEditor<Gd<ItemList>>,
    #[export]
    card_tabs_container: OnEditor<Gd<TabContainer>>,
    #[export]
    import_cards_file_dialog: OnEditor<Gd<FileDialog>>,
    #[export]
    unparsed_count_label: OnEditor<Gd<Label>>,
    #[export]
    card_filter_edit: OnEditor<Gd<LineEdit>>,
    #[export]
    parsed_filter_check: OnEditor<Gd<CheckButton>>,
    #[export]
    unparsed_filter_check: OnEditor<Gd<CheckButton>>,
    #[export]
    used_filter_check_node: OnEditor<Gd<CheckButton>>,
    #[export]
    apply_filter_button: OnEditor<Gd<Button>>,
    #[export]
    manage_cards_button: OnEditor<Gd<Button>>,
    #[export]
    cards_manager_window_node: OnEditor<Gd<CardsManagerWindowNode>>,
    #[export]
    import_error_dialog_node: OnEditor<Gd<AcceptDialog>>,
}

#[godot_api]
impl IControl for CardsTabNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.cards_list.clear();
        self.card_tabs_container
            .get_tab_bar()
            .unwrap()
            .set_tab_close_display_policy(CloseButtonDisplayPolicy::SHOW_ALWAYS);

        self.close_card_tabs();
    }
}

#[derive(Deserialize)]
struct ImportedCard {
    name: String,
    text: String,
}

struct CardFilter {
    filter: String,
    used_only: bool,
    allow_parsed: bool,
    allow_unparsed: bool,
}

impl CardFilter {
    fn empty() -> CardFilter {
        CardFilter {
            allow_parsed: true,
            allow_unparsed: true,
            filter: String::from(""),
            used_only: true,
        }
    }

    fn allows(&self, card: &CardModel, ph: &Option<&ParsingHistory>) -> bool {
        if self.used_only && !card.used {
            return false;
        }
        if let Some(parsing_history) = ph {
            let is_parsed = parsing_history.card_scripts.contains_key(&card.id);
            if is_parsed && !self.allow_parsed {
                return false;
            }
            if !is_parsed && !self.allow_unparsed {
                return false;
            }
        }
        if card.name.to_lowercase().contains(&self.filter) {
            return true;
        }
        if card.text.to_lowercase().contains(&self.filter) {
            return true;
        }

        return false;
    }
}

// private methods
impl CardsTabNode {
    fn connect_signals(&mut self) {
        self.import_cards_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_import_cards_button_pressed);
        self.import_cards_file_dialog
            .signals()
            .file_selected()
            .connect_other(self, Self::on_import_cards_file_dialog_file_selected);
        self.cards_list
            .signals()
            .item_activated()
            .connect_other(self, Self::on_cards_list_item_activated);
        self.card_tabs_container
            .get_tab_bar()
            .unwrap()
            .signals()
            .tab_close_pressed()
            .connect_other(self, Self::on_card_tabs_container_close_pressed);
        self.apply_filter_button
            .signals()
            .pressed()
            .connect_other(self, Self::apply_filters);
        self.manage_cards_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_manage_cards_button_pressed);
        self.cards_manager_window_node
            .signals()
            .cancel_request()
            .connect_other(self, Self::on_cards_manager_window_node_cancel_request);
        self.cards_manager_window_node
            .signals()
            .save_request()
            .connect_other(self, Self::on_cards_manager_window_node_save_request);
    }

    fn close_card_tabs(&mut self) {
        while self.card_tabs_container.get_child_count() > 0
            && let Some(node) = self.card_tabs_container.get_child(0)
        {
            self.card_tabs_container.remove_child(&node);
        }
    }

    fn construct_filter(&mut self) -> CardFilter {
        CardFilter {
            allow_parsed: self.parsed_filter_check.is_pressed(),
            allow_unparsed: self.unparsed_filter_check.is_pressed(),
            filter: self.card_filter_edit.get_text().to_lower().to_string(),
            used_only: self.used_filter_check_node.is_pressed(),
        }
    }

    fn apply_filters(&mut self) {
        let filter = self.construct_filter();
        self.reload_cards(&filter);
    }

    fn display_import_error(&mut self, err_msg: &str) {
        self.import_error_dialog_node.set_text(err_msg);
        self.import_error_dialog_node.show();
    }

    fn on_card_tabs_container_close_pressed(&mut self, idx: i64) {
        let child = self
            .card_tabs_container
            .get_child(idx.try_into().unwrap())
            .expect("Tried to close a non-existant card tab");
        self.card_tabs_container.remove_child(&child);
    }

    fn on_cards_list_item_activated(&mut self, idx: i64) {
        let card_id: i32 = self
            .cards_list
            .get_item_metadata(idx.try_into().unwrap())
            .to::<i32>();
        self.open_card(card_id);
    }

    fn on_import_cards_file_dialog_file_selected(&mut self, path: GString) {
        let project_name = self.loaded_project_name.to_string();
        let contents = match fs::read_to_string(path.to_string()) {
            Ok(text) => text,
            Err(err) => {
                self.display_import_error(&format!(
                    "Failed to import cards from {}\n{}",
                    &path, err
                ));
                return;
            }
        };
        let cards: Vec<ImportedCard> = match serde_json::from_str(contents.as_str()) {
            Ok(data) => data,
            Err(err) => {
                self.display_import_error(&format!(
                    "Incorrect card data format in {}\n{}",
                    &path, err
                ));
                return;
            }
        };

        let mut added_cards = Vec::<String>::new();
        let mut skipped_cards = Vec::<String>::new();

        let repo = self.repo.bind_mut();
        for card in cards {
            match repo.insert_card(&CardModel {
                id: -1,
                name: card.name.to_string(),
                text: card.text,
                project_name: project_name.to_string(),
                used: true,
            }) {
                Ok(()) => {
                    // logs.log(format!("Adding card {}", &card.name));
                    added_cards.push(card.name);
                }
                Err(e) => {
                    // logs.log(format!("Card {} already present, skipping it", &c.name));
                    godot_print!("import error: {}", e);
                    skipped_cards.push(card.name);
                }
            }
        }
        drop(repo);

        let mut logs = self.logs_tab.bind_mut();
        logs.log(format!(
            "Imported {} cards from {}",
            added_cards.len(),
            path.to_string()
        ));
        for card_name in added_cards {
            logs.log(format!(
                "Added card {}",
                LogsTabNode::format_card_name(&card_name)
            ));
        }
        for card_name in skipped_cards {
            logs.log(format!(
                "Skipped card {}, as it already exists",
                LogsTabNode::format_card_name(&card_name)
            ));
        }
        drop(logs);

        self.reload_cards(&CardFilter::empty());
    }

    fn on_import_cards_button_pressed(&mut self) {
        self.import_cards_file_dialog.show();
    }

    fn reload_cards(&mut self, filter: &CardFilter) {
        self.cards_list.clear();

        let project_name = self.loaded_project_name.to_string();
        let cards = self
            .repo
            .bind_mut()
            .get_cards(&project_name)
            .expect("Failed to load cards for project");
        let mut logs = self.logs_tab.bind_mut();
        logs.log(format!(
            "Loaded {} card(s)",
            LogsTabNode::format_count(cards.len())
        ));
        drop(logs);

        let repo = self.repo.bind_mut();
        let ph = repo.get_parsing_history(&self.loaded_project_name);

        self.cards_list.clear();
        for card in &cards {
            if !filter.allows(card, &ph) {
                continue;
            }

            let idx = self.cards_list.add_item(&card.name);
            self.cards_list
                .set_item_metadata(idx, &card.id.to_variant());
        }

        self.unparsed_count_label
            .set_text(cards.len().to_string().as_str());
    }
}

// signal connections
impl CardsTabNode {
    fn on_manage_cards_button_pressed(&mut self) {
        let cards = self
            .repo
            .bind_mut()
            .get_cards(&self.loaded_project_name)
            .expect("Failed to get cards");
        self.cards_manager_window_node.bind_mut().load_cards(cards);
        self.cards_manager_window_node.show();
    }

    fn on_cards_manager_window_node_cancel_request(&mut self) {
        self.cards_manager_window_node.hide();
    }

    fn on_cards_manager_window_node_save_request(&mut self) {
        self.cards_manager_window_node.hide();

        let (new_used_ids, new_unused_ids) =
            self.cards_manager_window_node.bind_mut().get_card_changes();
        let repo = self.repo.bind_mut();
        for id in new_used_ids {
            let mut card = repo.get_card(id).unwrap().unwrap();
            card.used = true;
            self.logs_tab.bind_mut().log(format!(
                "Card {} is now used in parsing",
                LogsTabNode::format_card_name(&card.name)
            ));
            repo.update_card_by_id(&card)
                .expect("Failed to update card to be used");
        }
        for id in new_unused_ids {
            let mut card = repo.get_card(id).unwrap().unwrap();
            card.used = false;
            self.logs_tab.bind_mut().log(format!(
                "Card {} is now not used in parsing",
                LogsTabNode::format_card_name(&card.name)
            ));
            repo.update_card_by_id(&card)
                .expect("Failed to update card to be unused");
        }
    }
}

// public methods
impl CardsTabNode {
    pub fn close_active_tab(&mut self) {
        let tab_idx = self.card_tabs_container.get_current_tab();
        if tab_idx == -1 {
            return;
        }

        let child = self.card_tabs_container.get_child(tab_idx).unwrap();
        self.card_tabs_container.remove_child(&child);
    }

    pub fn load_project(&mut self, project: &ProjectModel) {
        self.loaded_project_name = project.name.to_string();
        self.close_card_tabs();
        self.reload_cards(&CardFilter::empty());
    }

    pub fn open_card(&mut self, card_id: i32) {
        let prev_child_count = self.card_tabs_container.get_child_count();

        let card = self
            .repo
            .bind_mut()
            .get_card(card_id)
            .expect("Failed to load card")
            .expect("Tried to open a card tab with a card that doesnt exist");

        for i in 0..=(prev_child_count - 1) {
            let child = self
                .card_tabs_container
                .get_child(i)
                .expect("Failed to get child while iterating over get_children");
            if child.get_name().to_string() != card.name {
                continue;
            }

            self.card_tabs_container.set_current_tab(i);
            return;
        }

        let mut node = self.card_tab_scene.instantiate_as::<CardTabNode>();
        node.set_name(&card.name);

        self.card_tabs_container.add_child(&node);
        self.card_tabs_container.set_current_tab(prev_child_count);

        node.bind_mut().load_card(&card);
        node.bind_mut().update_parsing_history(
            self.repo
                .bind_mut()
                .get_parsing_history(&self.loaded_project_name),
        );
    }

    pub fn update_parsing_history(&mut self, ph: &ParsingHistory) {
        for i in 0..self.card_tabs_container.get_child_count() {
            let mut child = self
                .card_tabs_container
                .get_child(i)
                .expect("Failed to get child while iterating over get_children")
                .try_cast::<CardTabNode>()
                .unwrap();

            child.bind_mut().update_parsing_history(Some(ph));
        }
    }
}
