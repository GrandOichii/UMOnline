use std::cell::OnceCell;

use godot::classes::*;
use godot::prelude::*;

use crate::model::project::ProjectModel;
use crate::nodes::project_editor::ProjectEditorNode;
use crate::nodes::project_tabs::logs_tab::LogsTabNode;
use crate::repo::SQLiteParserRepository;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct CardsTabNode {
    base: Base<Control>,

    pub repo: OnceCell<Gd<SQLiteParserRepository>>,
    pub logs_tab: OnceCell<Gd<LogsTabNode>>,

    #[export_group(name = "Nodes")]
    #[export]
    import_cards_button: OnEditor<Gd<Button>>,
}

#[godot_api]
impl IControl for CardsTabNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl CardsTabNode {
    fn connect_signals(&mut self) {
        self.import_cards_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_import_cards_button_pressed);
    }

    fn on_import_cards_button_pressed(&mut self) {
        godot_print!("Import cards!");
    }

    fn get_repo(&mut self) -> &mut Gd<SQLiteParserRepository> {
        self.repo.get_mut().expect("repo was not initialized!")
    }

    fn get_logs_tab(&mut self) -> &mut Gd<LogsTabNode> {
        self.logs_tab.get_mut().expect("logs_tab was not initialized!")
    }

    pub fn load_project(&mut self, project: &ProjectModel) {
        let repo = self.get_repo();
        let cards = repo
            .bind_mut()
            .get_cards_from_project(&project.name)
            .expect("Failed to load cards for project");
        let mut logs = self.get_logs_tab().bind_mut();
        logs.log(format!("Loaded {} cards", LogsTabNode::format_count(cards.len())));
        godot_print!("Found {} cards for project {}", cards.len(), &project.name);
    }
}
