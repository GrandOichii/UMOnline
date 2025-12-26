use godot::classes::*;
use godot::prelude::*;

#[derive(GodotClass)]
#[class(init,base=RichTextLabel)]
pub struct ColoredTextNode {
    base: Base<RichTextLabel>,

    #[export]
    normal_color: Color,
    #[export]
    highlight_color: Color,
    //#[export_group(name="Nodes")]
}

#[godot_api]
impl IRichTextLabel for ColoredTextNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

impl ColoredTextNode {
    fn connect_signals(&mut self) {}

    fn colored(str: &str, color: &Color) -> String {
        format!("[color={}]{}[/color]", color.to_html(), str)
    }

    pub fn load_text(&mut self, full_text: String, highlight: String) {
        let text = ColoredTextNode::colored(
            full_text
                .replace(
                    highlight.as_str(),
                    ColoredTextNode::colored(&highlight, &self.highlight_color).as_str(),
                )
                .as_str(),
            &self.normal_color,
        );
        self.base_mut().set_text(text.as_str());
    }
}
