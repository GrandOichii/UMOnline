use chrono;
use godot::classes::*;
use godot::prelude::*;

use crate::model::project::ProjectModel;

#[derive(GodotClass)]
#[class(init,base=Control)]
pub struct LogsTabNode {
    base: Base<Control>,

    #[export_group(name = "Nodes")]
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

    fn format_msg(msg: String, color: String) -> String {
        format!(
            "[{}] [color={}]{}[/color]\n",
            chrono::offset::Local::now().format("%Y-%m-%d %H:%M:%S"),
            color,
            &msg
        )
    }

    pub fn log(&mut self, msg: String) {
        self.logs_label
            .append_text(LogsTabNode::format_msg(msg, "white".to_string()).as_str());
    }

    pub fn log_important(&mut self, msg: String) {
        self.logs_label
            .append_text(LogsTabNode::format_msg(msg, "cyan".to_string()).as_str());
    }

    pub fn load_project(&mut self, project: &ProjectModel) {
        self.log(format!(
            "Loaded project {}",
            LogsTabNode::format_project_name(&project.name)
        ));
    }

    pub fn format_project_name(project_name: &String) -> String {
        format!("[color=red]{}[/color]", project_name)
    }

    pub fn format_count(count: usize) -> String {
        format!("[color=orange]{}[/color]", count)
    }

    pub fn format_card_name(card_name: &String) -> String {
        format!("[color=green]{}[/color]", card_name)
    }

    pub fn format_failed_to_parse_card_name(card_name: &String) -> String {
        format!("[color=red]{}[/color]", card_name)
    }
}
