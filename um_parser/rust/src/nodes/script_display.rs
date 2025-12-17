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
    return_color: Color,
    #[export]
    string_color: Color,
    #[export]
    table_name_color: Color,
    #[export]
    number_color: Color,
    #[export]
    true_color: Color,
    #[export]
    false_color: Color,
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
            format.replace(
                "{}",
                format!("[color={}]$1[/color]", color.to_html()).as_str(),
            ),
        )
        .to_string()
    }

    fn format_script(&self, script: &String) -> String {
        let simple_replace_table = vec![
            ("nil", &self.nil_color),
            ("end", &self.end_color),
            ("function", &self.function_color),
            ("return", &self.return_color),
            ("true", &self.true_color),
            ("false", &self.false_color),
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
            (r"(\w+)\.", "{}.", &self.table_name_color),
            (r"(\w+\s*):", "{}:", &self.table_name_color),
            (r"\b([0-9]+)\b", "{}", &self.number_color),

            ("('.+')", "{}", &self.string_color),
            ("(\".+\")", "{}", &self.string_color),
        ];
        for chunk in chunks {
            result = self.replace_chunks(&result, chunk.1, Regex::new(chunk.0).unwrap(), chunk.2);
        }

        // fix strings
        let string_re = Regex::new("(\".+\")").unwrap();
        let result_copy = result.to_string();
        let matches = string_re.captures(&result_copy);
        for (i, m) in matches.iter().enumerate() {
            let original = m.get(1).unwrap().as_str();
            let start_color_bbc_re = Regex::new(r"(\[color=.+?\]|\[\/color\])").unwrap();
            let replace = start_color_bbc_re.replace_all(original, "").to_string();
            result = result.replace(original, &replace);
        }
        return result.to_string();
    }
}
