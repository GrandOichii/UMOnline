use godot::classes::*;
use godot::prelude::*;

use crate::model::project::ProjectModel;
use crate::repo::ParserRepository;
use crate::repo::SQLiteParserRepository;

#[derive(GodotClass)]
#[class(init,base=Control)]
struct RootNode {
    base: Base<Control>,

    #[export]
    repo: OnEditor<Gd<SQLiteParserRepository>>,
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
}

#[godot_api]
impl IControl for RootNode {
    fn ready(&mut self) {
        self.toggle_project_ui(false);
        self.project_info_container.set_visible(false);

        // -== connect signals ==-
        // project_list.item_selected
        self.project_list
            .signals()
            .item_selected()
            .connect_other(self, RootNode::on_project_list_item_selected);
        // project_list.item_activated
        self.project_list
            .signals()
            .item_activated()
            .connect_other(self, RootNode::on_project_list_item_activated);
        // create_button.pressed
        self.create_button
            .signals()
            .pressed()
            .connect_other(self, RootNode::on_create_button_pressed);
        // edit_button.pressed
        self.edit_button
            .signals()
            .pressed()
            .connect_other(self, RootNode::on_edit_button_pressed);
        // delete_button.pressed
        self.delete_button
            .signals()
            .pressed()
            .connect_other(self, RootNode::on_delete_button_pressed);

        self.fill_project_names();
    }
}

impl RootNode {
    fn selected_project(&mut self) -> ProjectModel {
        let idx = self.project_list.get_selected_items()[0];
        let project_name: String = self.project_list.get_item_metadata(idx).to();
        godot_print!("{project_name}");

        self.repo.bind_mut().get_project(&project_name)
            .expect("Failed to get projects from DB")
            .expect("Failed to find project")
    }

    fn toggle_project_ui(&mut self, v: bool) {
        self.edit_button.set_disabled(!v);
        self.delete_button.set_disabled(!v);
        self.project_info_container.set_visible(v);
    }

    fn on_create_button_pressed(&mut self) {
        godot_print!("CREATE PROJECT");
        // TODO
    }

    fn on_edit_button_pressed(&mut self) {
        self.edit_active_project();
    }

    fn on_delete_button_pressed(&mut self) {
        let selected = self.selected_project();
        godot_print!("DELETE PROJECT {}", selected.name);
        // TODO
    }

    fn edit_active_project(&mut self) {
        let selected = self.selected_project();
        godot_print!("EDIT PROJECT {}", selected.name);
        // TODO
    }

    fn on_project_list_item_activated(&mut self, idx: i64) {
        self.edit_active_project();
    }

    fn on_project_list_item_selected(&mut self, idx: i64) {
        let project_name: String = self.project_list
            .get_item_metadata(idx.try_into().unwrap())
            .to();

        self.toggle_project_ui(true);

        godot_print!("Activated {project_name}");

        let project = self.repo.bind_mut().get_project(&project_name)
            .expect("Failed to get project from DB")
            .expect("Somehow clicked non-existant project in project_list");

        self.project_name_label.set_text(&project.name);
        self.project_description_label.set_text(&project.description);
    }

    fn fill_project_names(&mut self) {
        let projects = self.repo.bind_mut().get_projects()
            .expect("Failed to get projects from DB");

        self.project_list.clear();

        for project in projects {
            let idx = self.project_list.add_item(&project.name);
            self.project_list.set_item_metadata(idx, &project.name.to_variant());
        }
    }
}
