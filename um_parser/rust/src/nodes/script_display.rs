use godot::classes::*;
use godot::prelude::*;
use regex::Regex;

#[derive(GodotClass)]
#[class(init,base=RichTextLabel)]
pub struct ScriptDisplayNode {
    base: Base<RichTextLabel>,

    #[export]
    nil_color: Color,
    #[export]
    function_color: Color,
    #[export]
    end_color: Color,
    #[export]
    function_name_color: Color,
    #[export]
    string_color: Color,
}

#[godot_api]
impl IRichTextLabel for ScriptDisplayNode {
    fn ready(&mut self) {
        self.connect_signals();

        self.base_mut().set_use_bbcode(true);
    }
}

impl ScriptDisplayNode {
    fn connect_signals(&mut self) {}

    pub fn set_script_text(&mut self, script: &String) {
        let formatted = self.format_script(script);
        self.base_mut().set_text(&formatted);
    }

    fn replace_chunks(&self, script: &String, format: &str, re: Regex, color: &Color) -> String {
        re.replace_all(
            script,
            // "amogus $1"
            format.replace(
                "{}",
                format!("[color={}]$1[/color]", color.to_html()).as_str(),
            ),
        )
        .to_string()
        // let mut result = script.to_string();
        // while let Some(captures) = re.captures(&result) {

        // }
        // return result;
    }

    fn format_script(&self, script: &String) -> String {
        let simple_replace_table = vec![
            ("nil", &self.nil_color),
            ("end", &self.end_color),
            ("function", &self.function_color),
        ];
        let mut result = script.to_string();
        for pair in simple_replace_table {
            let re = Regex::new(format!(r"\b{}\b", pair.0).as_str()).unwrap();
            result = re
                .replace_all(
                    &result,
                    format!("[color={}]{}[/color]", pair.1.to_html(), pair.0),
                )
                .to_string();
        }

        let chunks = vec![
            (r"(\w+)\(", "{}(", &self.function_name_color),
            ("('.+')", "{}", &self.string_color),
            ("(\".+\")", "{}", &self.string_color),
        ];
        for chunk in chunks {
            result = self.replace_chunks(&result, chunk.1, Regex::new(chunk.0).unwrap(), chunk.2);
        }
        return result.to_string();
    }
}
