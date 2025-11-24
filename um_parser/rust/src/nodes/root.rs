use godot::classes::*;
use godot::prelude::*;

use crate::nodes::project_editor::ProjectEditorNode;
use crate::nodes::project_list::ProjectListNode;

#[derive(GodotClass)]
#[class(init,base=Control)]
struct RootNode {
    base: Base<Control>,

    #[export_group(name = "Nodes")]
    #[export]
    project_list_node: OnEditor<Gd<ProjectListNode>>,
    #[export]
    project_editor_node: OnEditor<Gd<ProjectEditorNode>>,
}

#[godot_api]
impl IControl for RootNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl RootNode {
    fn connect_signals(&mut self) {
        self.project_list_node
            .signals()
            .edit_project_request()
            .connect_other(self, Self::on_edit_project_request);
    }

    fn on_edit_project_request(&mut self, project_name: GString) {
        self.project_list_node.hide();
        self.project_editor_node.show();
        self.project_editor_node
            .bind_mut()
            .load_project(&project_name.to_string());
    }
}
