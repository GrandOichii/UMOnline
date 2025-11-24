use godot::classes::*;
use godot::prelude::*;

use crate::model::project::ProjectModel;
use crate::repo::ParserRepository;
use crate::repo::SQLiteParserRepository;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct ProjectListNode {
    base: Base<Control>,

    #[export]
    repo: OnEditor<Gd<SQLiteParserRepository>>,
    #[export_group(name = "Nodes")]
    #[export]
    project_list: OnEditor<Gd<ItemList>>,
    #[export]
    project_info_container: OnEditor<Gd<Container>>,
    #[export]
    project_name_label: OnEditor<Gd<Label>>,
    #[export]
    project_description_label: OnEditor<Gd<RichTextLabel>>,
    #[export]
    create_button: OnEditor<Gd<Button>>,
    #[export]
    edit_button: OnEditor<Gd<Button>>,
    #[export]
    delete_button: OnEditor<Gd<Button>>,
    #[export]
    delete_confirmation_dialog: OnEditor<Gd<ConfirmationDialog>>,
    #[export]
    new_project_name_edit: OnEditor<Gd<LineEdit>>,
    #[export]
    new_project_description_edit: OnEditor<Gd<TextEdit>>,
    #[export]
    name_taken_dialog: OnEditor<Gd<AcceptDialog>>,
}

#[godot_api]
impl ProjectListNode {
    #[signal]
    pub fn edit_project_request(project_name: GString);
}

#[godot_api]
impl IControl for ProjectListNode {
    fn ready(&mut self) {
        self.toggle_project_ui(false);
        self.project_info_container.set_visible(false);

        // open previous project
        self.open_last_project();

        // -== connect signals ==-
        // project_list.item_selected
        self.project_list
            .signals()
            .item_selected()
            .connect_other(self, ProjectListNode::on_project_list_item_selected);
        // project_list.item_activated
        self.project_list
            .signals()
            .item_activated()
            .connect_other(self, ProjectListNode::on_project_list_item_activated);
        // create_button.pressed
        self.create_button
            .signals()
            .pressed()
            .connect_other(self, ProjectListNode::on_create_button_pressed);
        // edit_button.pressed
        self.edit_button
            .signals()
            .pressed()
            .connect_other(self, ProjectListNode::on_edit_button_pressed);
        // delete_button.pressed
        self.delete_button
            .signals()
            .pressed()
            .connect_other(self, Self::on_delete_button_pressed);
        self.delete_confirmation_dialog
            .signals()
            .confirmed()
            .connect_other(self, Self::on_delete_confirmation_accept);
        self.delete_confirmation_dialog
            .signals()
            .canceled()
            .connect_other(self, Self::on_delete_confirmation_cancel);
        // self.create_project_window
        //     .signals()
        //     .submitted()
        //     .connect_other(self, Self::on_create_project_submitted);

        self.fill_project_names();

        // self.create_project_window.hide();
        self.delete_confirmation_dialog.hide();
    }
}

impl ProjectListNode {
    fn open_last_project(&mut self) {
        let mut repo = self.repo.bind_mut();
        let editor = repo
            .get_editor()
            .expect("Failed to load editor config from DB");

        let project = repo
            .get_project(&editor.last_project_name)
            .expect("Failed to read last opened project from DB");

        match project {
            Some(p) => godot_print!("Opening last project: {}", &p.name),
            None => godot_print!("Failed to find last project: {}", &editor.last_project_name),
        };
    }

    fn on_delete_confirmation_accept(&mut self) {
        let project = self.selected_project();
        self.repo
            .bind_mut()
            .delete_project(&project.name)
            .expect("Failed to delete project");

        self.fill_project_names();
    }

    fn on_delete_confirmation_cancel(&mut self) {}

    fn selected_project(&mut self) -> ProjectModel {
        let idx = self.project_list.get_selected_items()[0];
        let project_name: String = self.project_list.get_item_metadata(idx).to();

        self.repo
            .bind_mut()
            .get_project(&project_name)
            .expect("Failed to get projects from DB")
            .expect("Failed to find project")
    }

    fn toggle_project_ui(&mut self, v: bool) {
        self.edit_button.set_disabled(!v);
        self.delete_button.set_disabled(!v);
        self.project_info_container.set_visible(v);
    }

    fn on_create_button_pressed(&mut self) {
        let name = self.new_project_name_edit.get_text().to_string();
        if name.len() == 0 {
            return;
        }
        
        let project = ProjectModel {
            description: self.new_project_description_edit.get_text().to_string(),
            name: name,
        };

        let existing = self
            .repo
            .bind_mut()
            .get_project(&project.name)
            .expect("Failed to fetch project from DB");

        match existing {
            Some(_) => {
                self.name_taken_dialog.show();
            }
            None => {
                self.repo
                    .bind_mut()
                    .insert_project(&project)
                    .expect("Failed to insert project to DB");
                self.fill_project_names();
                self.new_project_name_edit.clear();
                self.new_project_description_edit.clear();
            }
        }
    }

    fn on_edit_button_pressed(&mut self) {
        self.edit_active_project();
    }

    fn on_delete_button_pressed(&mut self) {
        let selected = self.selected_project();
        self.delete_confirmation_dialog
            .set_text(format!("Delete project {}", &selected.name).as_str());
        self.delete_confirmation_dialog.show();
    }

    fn edit_active_project(&mut self) {
        let selected = self.selected_project();
        self.signals().edit_project_request().emit(&selected.name);
    }

    fn on_project_list_item_activated(&mut self, idx: i64) {
        self.edit_active_project();
    }

    fn on_project_list_item_selected(&mut self, idx: i64) {
        let project_name: String = self
            .project_list
            .get_item_metadata(idx.try_into().unwrap())
            .to();

        self.toggle_project_ui(true);

        let project = self
            .repo
            .bind_mut()
            .get_project(&project_name)
            .expect("Failed to get project from DB")
            .expect("Somehow clicked non-existant project in project_list");

        self.project_name_label.set_text(&project.name);
        self.project_description_label
            .set_text(&project.description);
    }

    fn fill_project_names(&mut self) {
        let projects = self
            .repo
            .bind_mut()
            .get_projects()
            .expect("Failed to get projects from DB");

        self.project_list.clear();

        for project in projects {
            let idx = self.project_list.add_item(&project.name);
            self.project_list
                .set_item_metadata(idx, &project.name.to_variant());
        }
    }
}
