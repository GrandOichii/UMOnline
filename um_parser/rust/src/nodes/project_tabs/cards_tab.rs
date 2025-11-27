use std::cell::OnceCell;
use std::fs;

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
}

#[godot_api]
impl IControl for CardsTabNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.cards_list.clear();

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
    }

    fn on_cards_list_item_activated(&mut self, idx: i64) {
        let card_name = self.cards_list.get_item_text(idx.try_into().unwrap());

        self.open_card(&card_name.to_string());
    }

    fn open_card(&mut self, card_name: &String) {
        godot_print!("Activated card {}", &card_name);
        // TODO iterate over all opened tabs
        // TODO ... if any match, focus on that tab
        let prev_child_count = self.card_tabs_container.get_child_count();

        for i in 0..=(prev_child_count-1) {
            let child = self.card_tabs_container.get_child(i)
                .expect("Failed to get child while iterating over get_children");
            if child.get_name().to_string() != *card_name {
                continue
            }

            self.card_tabs_container.set_current_tab(i);
            return;
        }
        // for child in self.card_tabs_container.get_children()
        let mut node = self.card_tab_scene.instantiate_as::<CardTabNode>();
        node.set_name(card_name);

        self.card_tabs_container.add_child(&node);
        self.card_tabs_container.set_current_tab(prev_child_count);
    }

    fn on_import_cards_file_dialog_file_selected(&mut self, path: GString) {
        let project_name = self.loaded_project_name.to_string();
        let contents =
            fs::read_to_string(path.to_string()).expect("Failed to find file with cards");
        let cards: Vec<ImportedCard> =
            serde_json::from_str(contents.as_str()).expect("Failed to parse cards json"); // TODO better error handling here

        let mut added_cards = Vec::<String>::new();
        let mut skipped_cards = Vec::<String>::new();

        let mut repo = self.get_repo().bind_mut();
        for card in cards {
            match repo
                .get_card(&card.name)
                .expect("Failed to read card from DB")
            {
                Some(c) => {
                    // logs.log(format!("Card {} already present, skipping it", &c.name));
                    skipped_cards.push(c.name);
                }
                None => {
                    // logs.log(format!("Adding card {}", &card.name));
                    added_cards.push(card.name.to_string());
                    repo.insert_card(&CardModel {
                        name: card.name,
                        text: card.text,
                        project_name: project_name.to_string(),
                    })
                    .expect("Failed to insert card into DB");
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

        self.reload_cards();
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
        self.reload_cards();
    }

    fn reload_cards(&mut self) {
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
        for card in cards {
            self.cards_list.add_item(&card.name);
        }
    }
}

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardTabNode {
    base: Base<Control>,
    //#[export_group(name="Nodes")]
}

#[godot_api]
impl IControl for CardTabNode {
    fn ready(&mut self) {}
}
