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

    pub repo: OnceCell<Gd<SQLiteParserRepository>>,
    pub logs_tab: OnceCell<Gd<LogsTabNode>>,
    loaded_project_name: String,

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
        logs.log(format!("Imported {} cards from {}", added_cards.len(), path.to_string()));
        for card_name in added_cards {
            logs.log(format!("Added card {}", LogsTabNode::format_card_name(&card_name)));
        }
        for card_name in skipped_cards {
            logs.log(format!("Skipped card {}, as it already exists", LogsTabNode::format_card_name(&card_name)));
        }
        drop(logs);

        self.reload_cards();
    }

    fn on_import_cards_button_pressed(&mut self) {
        self.import_cards_file_dialog.show();
    }

    fn get_repo(&mut self) -> &mut Gd<SQLiteParserRepository> {
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
