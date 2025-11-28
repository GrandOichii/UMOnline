use std::cell::OnceCell;
use std::fs;

use godot::classes::tab_bar::CloseButtonDisplayPolicy;
use godot::classes::*;
use godot::prelude::*;
use serde::Deserialize;

use crate::model::card::CardModel;
use crate::model::project::ProjectModel;
use crate::nodes::project_tabs::logs_tab::LogsTabNode;
use crate::repo::*;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardsTabNode {
    base: Base<Control>,

    pub repo: OnceCell<Gd<ParserRepositoryNode>>,
    pub logs_tab: OnceCell<Gd<LogsTabNode>>,
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
    apply_filter_button: OnEditor<Gd<Button>>,
}

#[godot_api]
impl IControl for CardsTabNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.cards_list.clear();
        self.card_tabs_container.get_tab_bar().unwrap().set_tab_close_display_policy(CloseButtonDisplayPolicy::SHOW_ALWAYS);

        while self.card_tabs_container.get_child_count() > 0
            && let Some(node) = self.card_tabs_container.get_child(0)
        {
            self.card_tabs_container.remove_child(&node);
        }
    }
}

#[derive(Deserialize)]
struct ImportedCard {
    name: String,
    text: String,
}

struct CardFilter {
    filter: String,
    allow_parsed: bool,
    allow_unparsed: bool,
}

impl CardFilter {
    fn empty() -> CardFilter {
        CardFilter {
            allow_parsed: true,
            allow_unparsed: true,
            filter: String::from(""),
        }
    }

    fn allows(&self, card: &CardModel) -> bool {
        if card.name.to_lowercase().contains(&self.filter) {
            return true;
        }
        if card.text.to_lowercase().contains(&self.filter) {
            return true;
        }
        // TODO allow_parsed and allow_unparsed

        return false;
    }
}

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
        self.card_tabs_container.get_tab_bar().unwrap()
            .signals()
            .tab_close_pressed()
            .connect_other(self, Self::on_card_tabs_container_close_pressed);
        self.apply_filter_button
            .signals()
            .pressed()
            .connect_other(self, Self::apply_filters);
    }

    fn construct_filter(&mut self) -> CardFilter {
        CardFilter{
            allow_parsed: self.parsed_filter_check.is_pressed(),
            allow_unparsed: self.unparsed_filter_check.is_pressed(),
            filter: self.card_filter_edit.get_text().to_lower().to_string(),
        }
    }

    fn apply_filters(&mut self) {
        let filter = self.construct_filter();
        self.reload_cards(&filter);
    }

    fn on_card_tabs_container_close_pressed(&mut self, idx: i64) {
        let child = self.card_tabs_container.get_child(idx.try_into().unwrap())
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

    fn open_card(&mut self, card_id: i32) {
        godot_print!("Activated card {}", card_id);
        let prev_child_count = self.card_tabs_container.get_child_count();

        let card = self.get_repo().bind_mut().get_card(card_id)
            .expect("Failed to load card").expect("Tried to open a card tab with a card that doesnt exist");

        for i in 0..=(prev_child_count-1) {
            let child = self.card_tabs_container.get_child(i)
                .expect("Failed to get child while iterating over get_children");
            if child.get_name().to_string() != card.name {
                continue
            }

            self.card_tabs_container.set_current_tab(i);
            return;
        }

        let mut node = self.card_tab_scene.instantiate_as::<CardTabNode>();
        node.set_name(&card.name);

        self.card_tabs_container.add_child(&node);
        self.card_tabs_container.set_current_tab(prev_child_count);


        node.bind_mut().load_card(&card);
    }

    fn on_import_cards_file_dialog_file_selected(&mut self, path: GString) {
        let project_name = self.loaded_project_name.to_string();
        let contents =
            fs::read_to_string(path.to_string()).expect("Failed to find file with cards");
        let cards: Vec<ImportedCard> =
            serde_json::from_str(contents.as_str()).expect("Failed to parse cards json"); // TODO tell user that json is invalid

        let mut added_cards = Vec::<String>::new();
        let mut skipped_cards = Vec::<String>::new();

        let mut repo = self.get_repo().bind_mut();
        for card in cards {
            match repo.insert_card(&CardModel {
                id: -1,
                name: card.name.to_string(),
                text: card.text,
                project_name: project_name.to_string(),
            }) {
                Ok(()) => {
                    // logs.log(format!("Adding card {}", &card.name));
                    added_cards.push(card.name);
                }
                Err(e) => {
                    // logs.log(format!("Card {} already present, skipping it", &c.name));
                    skipped_cards.push(card.name);
                }
            }
        }
        drop(repo);

        let mut logs = self.get_logs_tab().bind_mut();
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

    fn get_repo(&mut self) -> &mut Gd<ParserRepositoryNode> {
        self.repo.get_mut().expect("repo was not initialized!")
    }

    fn get_logs_tab(&mut self) -> &mut Gd<LogsTabNode> {
        self.logs_tab
            .get_mut()
            .expect("logs_tab was not initialized!")
    }

    pub fn load_project(&mut self, project: &ProjectModel) {
        self.loaded_project_name = project.name.to_string();
        self.reload_cards(&CardFilter::empty());
    }

    fn reload_cards(&mut self, filter: &CardFilter) {
        self.cards_list.clear();
        
        let project_name = self.loaded_project_name.to_string();
        let repo = self.get_repo();
        let cards = repo
            .bind_mut()
            .get_cards_from_project(&project_name)
            .expect("Failed to load cards for project");
        let mut logs = self.get_logs_tab().bind_mut();
        logs.log(format!(
            "Loaded {} cards",
            LogsTabNode::format_count(cards.len())
        ));
        drop(logs);

        self.cards_list.clear();
        for card in &cards {
            if !filter.allows(card) {
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

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardTabNode {
    base: Base<Control>,
    #[export_group(name="Nodes")]
    #[export]
    name_label: OnEditor<Gd<Label>>,
    #[export]
    text_display: OnEditor<Gd<TextEdit>>,
}

#[godot_api]
impl IControl for CardTabNode {
    fn ready(&mut self) {}
}

impl CardTabNode {
    fn load_card(&mut self, card: &CardModel) {
        self.name_label.set_text(&card.name);
        self.text_display.set_text(&card.text);
    }
}
