use std::{cell::RefCell, rc::Rc};

use mlua::Lua;
use regex::Regex;

use crate::parsers::parser::*;

pub struct Matcher {
    pub script: String,
    pub pattern: Regex,
}

impl ParserNode {
    pub fn matcher(
        name: String,
        pattern: Regex,
        script: String,
        children: Vec<Rc<RefCell<ParserNode>>>,
    ) -> ParserNode {
        ParserNode {
            name: name,
            children: children,
            parser: Box::new(Matcher {
                pattern: pattern,
                script: script,
            }),
        }
    }
}

impl Parser for Matcher {
    fn parse(&self, text: &str, node: Rc<RefCell<ParserNode>>, lua: &Lua) -> ParseResult {
        let m = self.pattern.captures(text);
        let mut didnt_match = 0;
        if m.is_none() {
            return ParseResult {
                status: ParseResultStatus::DidntMatch,
                text: text.to_string(),
                parent: node,
                children: vec![],
                parse_data: lua
                    .create_table()
                    .expect("Failed to create arg table for matcher"),
            };
        }

        let table = lua
            .create_table()
            .expect("Failed to create arg table for matcher");

        for (i, group) in m.iter().enumerate() {
            if group.len() == 1 {
                continue;
            }
            table
                .set(i + 1, group.get(1).unwrap().as_str().to_string())
                // .set(i + 1, group.)
                .expect("Failed to set arg table for matcher");
        }

        let mut result = ParseResult {
            status: ParseResultStatus::Success,
            text: text.to_string(),
            parent: node.clone(),
            children: Vec::new(),
            parse_data: table,
        };
        let mut i = 1;
        if result.parent.borrow().children.len() == 0 {
            return result;
        }

        let n = node.borrow();
        for g in m.iter() {
            let child = n.children[i - 1].clone();
            let child_result = ParserNode::parse(child, g.get(1).unwrap().as_str(), lua);
            i += 1;
            if child_result.status != ParseResultStatus::Success {
                result.status = ParseResultStatus::ChildFailed;
            }
            if child_result.status == ParseResultStatus::DidntMatch {
                didnt_match += 1;
            }
            result.children.push(child_result);
        }
        if didnt_match == n.children.len() {
            result.status = ParseResultStatus::DidntMatch;
        }
        return result;
    }

    fn get_script(&self) -> String {
        self.script.to_string()
    }
}

#[cfg(test)]
mod tests {
    use regex::Regex;

    use super::*;

    #[test]
    fn matcher_test_success() {
        let root = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("matcher1"),
            Regex::new("Hello, (.+)").unwrap(),
            String::from("function _Create(text, children, data) return data[1] end"),
            vec![],
        )));

        let text = "Hello, something";

        let lua = Lua::new();
        let parse_result = ParserNode::parse(root, text, &lua);
        assert_eq!(parse_result.status, ParseResultStatus::Success);
        let script = parse_result
            .create_script(&lua)
            .expect("Failed to generate script");
        assert_eq!(script, "something");
    }

    #[test]
    fn matcher_test_didnt_match() {
        let root = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("matcher1"),
            Regex::new("Not Hello, (.+)").unwrap(),
            String::from("function _Create(text, children, data) return data[1] end"),
            vec![],
        )));

        let text = "Hello, something";

        let lua = Lua::new();
        let parse_result = ParserNode::parse(root, text, &lua);
        assert_eq!(parse_result.status, ParseResultStatus::DidntMatch);
    }
}
