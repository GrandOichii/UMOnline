use godot::classes::*;
use godot::prelude::*;
use chrono;

use crate::model::project::ProjectModel;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct LogsTabNode {
    base: Base<Control>,

    #[export_group(name="Nodes")]
    #[export]
    clear_button: OnEditor<Gd<Button>>,
    #[export]
    logs_label: OnEditor<Gd<RichTextLabel>>,
}

#[godot_api]
impl IControl for LogsTabNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.clear_logs();
    }
}

impl LogsTabNode {
    fn connect_signals(&mut self) {
        self.clear_button
            .signals()
            .pressed()
            .connect_other(self, Self::clear_logs);
    }

    pub fn clear_logs(&mut self) {
        self.logs_label.clear();
    }

    pub fn log(&mut self, msg: String) {
        self.logs_label.append_text(format!("[{}] {}\n", chrono::offset::Local::now().format("%Y-%m-%d %H:%M:%S"), &msg).as_str());

        godot_print!("{}", self.logs_label.get_text());
    }

    pub fn load_project(&mut self, project: &ProjectModel) {
        self.log(format!("Loaded project {}", LogsTabNode::format_project_name(&project.name)));
    }

    pub fn format_project_name(project_name: &String) -> String {
        format!("[color=red]{}[/color]", project_name)
    }

    pub fn format_count(count: usize) -> String {
        format!("[color=orange]{}[/color]", count)
    }
}