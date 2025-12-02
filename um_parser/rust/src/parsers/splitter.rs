use std::{cell::RefCell, rc::Rc};

use mlua::Lua;
use regex::Regex;

use crate::parsers::parser::*;

static SPLITTER_SCRIPT: &str = r#"
function _Create(text, children)
    local result = ''
    for i, child in ipairs(children) do
        if child ~= '' then
            if i ~= 1 then
                result = result..',\n'
            end
            result = result..child
        end
    end
    return result
end
"#;

pub struct Splitter {
    pub script: String,
    pub pattern: Regex,
}

impl ParserNode {
    pub fn splitter(
        name: String,
        pattern: Regex,
        children: Vec<Rc<RefCell<ParserNode>>>,
    ) -> ParserNode {
        ParserNode {
            name: name,
            parser: Box::new(Splitter {
                script: SPLITTER_SCRIPT.to_string(),
                pattern: pattern,
            }),
            children: children,
        }
    }

    pub fn new_with_script<'a>(
        name: String,
        pattern: Regex,
        script: String,
        children: Vec<Rc<RefCell<ParserNode>>>,
    ) -> ParserNode {
        ParserNode {
            name: name,
            parser: Box::new(Splitter {
                script: script,
                pattern: pattern,
            }),
            children: children,
        }
    }
}

impl Parser for Splitter {
    fn parse(&self, text: &str, node: Rc<RefCell<ParserNode>>, lua: &Lua) -> ParseResult {
        let split = self.pattern.split(text);
        let mut status = ParseResultStatus::Success;
        let mut children: Vec<ParseResult> = Vec::new();
        if node.borrow().children.len() == 0 {
            return ParseResult {
                status: ParseResultStatus::AllChildrenFailed,
                text: text.to_string(),
                parent: node,
                children: children,
                parse_data: lua
                    .create_table()
                    .expect("Failed to create arg table for splitter"),
            };
        }
        let child = node.borrow().children[0].clone();

        let mut split_count: usize = 0;
        let mut failed = 0;

        for part in split {
            if part.is_empty() {
                continue;
            }
            let part_result = ParserNode::parse(child.clone(), part, lua);
            let s = part_result.status;
            children.push(part_result);
            if s == ParseResultStatus::Success {
                continue;
            }
            failed += 1;
            split_count += 1;
        }

        if failed > 0 {
            status = ParseResultStatus::ChildFailed;
            if failed == split_count {
                status = ParseResultStatus::AllChildrenFailed;
            }
        }
        return ParseResult {
            status: status,
            text: text.to_string(),
            parent: node,
            children: children,
            parse_data: lua
                .create_table()
                .expect("Failed to create arg table for splitter"),
        };
    }

    fn get_script(&self) -> String {
        return self.script.to_string();
    }
}

#[cfg(test)]
mod tests {
    use regex::Regex;

    use super::*;

    #[test]
    fn selector_test_multiple() {
        let m1 = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("m1"),
            Regex::new("m1").unwrap(),
            String::from("function _Create(text, children, data) return 'MATCH' end"),
            vec![],
        )));
        let root = Rc::new(RefCell::new(ParserNode::splitter(
            String::from("selector1"),
            Regex::new(" ").unwrap(),
            vec![m1],
        )));

        let text = "m1 m1 m1";

        let lua = Lua::new();
        let parse_result = ParserNode::parse(root, text, &lua);
        assert_eq!(parse_result.status, ParseResultStatus::Success);
        let script = parse_result
            .create_script(&lua)
            .expect("Failed to generate script");
        assert_eq!(script, "MATCH,\nMATCH,\nMATCH");
    }

    #[test]
    fn selector_test_single() {
        let m1 = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("m1"),
            Regex::new("m1").unwrap(),
            String::from("function _Create(text, children, data) return 'MATCH' end"),
            vec![],
        )));
        let root = Rc::new(RefCell::new(ParserNode::splitter(
            String::from("selector1"),
            Regex::new(" ").unwrap(),
            vec![m1],
        )));

        let text = "m1";

        let lua = Lua::new();
        let parse_result = ParserNode::parse(root, text, &lua);
        assert_eq!(parse_result.status, ParseResultStatus::Success);
        let script = parse_result
            .create_script(&lua)
            .expect("Failed to generate script");
        assert_eq!(script, "MATCH");
    }
}
