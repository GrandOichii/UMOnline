use godot::classes::*;
use godot::prelude::*;

use crate::nodes::project_tabs::cards_tab::*;
use crate::nodes::project_tabs::logs_tab::LogsTabNode;
use crate::nodes::project_tabs::parsers_tab::ParsersTabNode;
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
}

#[godot_api]
impl IControl for ProjectEditorNode {
    fn ready(&mut self) {
        self.connect_signals();

        // cards tab
        self.cards_tab
            .bind_mut()
            .repo
            .set(self.repo.clone())
            .expect("Failed to pass down repo node");
        self.cards_tab
            .bind_mut()
            .logs_tab
            .set(self.logs_tab.clone())
            .expect("Failed to pass down logs_tab node");

        // parsers tab
        self.parsers_tab
            .bind_mut()
            .repo
            .set(self.repo.clone())
            .expect("Failed to pass down repo node");
        self.parsers_tab
            .bind_mut()
            .logs_tab
            .set(self.logs_tab.clone())
            .expect("Failed to pass down logs_tab node");
    }
}

impl ProjectEditorNode {
    fn connect_signals(&mut self) {
        self.project_description_edit
            .signals()
            .text_changed()
            .connect_other(self, Self::on_project_description_edit_text_changed);
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
