use std::{cell::RefCell, rc::Rc};

use mlua::Lua;

use crate::parsers::parser::*;

pub struct Selector;

pub const SELECTOR_SCRIPT: &'static str = "-- default selector script
function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '' then
            return child
        end
    end
    error(\"Shouldn't reach this point\")
end";

impl ParserNode {
    pub fn selector<'a>(name: String, children: Vec<Rc<RefCell<ParserNode>>>) -> ParserNode {
        ParserNode {
            name: name,
            parser: Box::new(Selector),
            children: children,
        }
    }
}

impl Parser for Selector {
    fn parse(&self, text: &str, node: Rc<RefCell<ParserNode>>, lua: &Lua) -> ParseResult {
        let mut children: Vec<ParseResult> = Vec::new();
        let mut closest_to_match_idx: Option<usize> = None;
        let mut all_didnt_match = true;
        let mut status = ParseResultStatus::AllChildrenFailed;
        for child in node.borrow().children.iter() {
            children.push(ParserNode::parse(child.clone(), text, lua));
        }
        for (idx, child) in children.iter().enumerate() {
            if child.status == ParseResultStatus::DidntMatch {
                continue;
            }

            all_didnt_match = false;

            if closest_to_match_idx.is_none() || child.status == ParseResultStatus::Success {
                closest_to_match_idx = Some(idx);
            }

            if child.status == ParseResultStatus::Success {
                status = ParseResultStatus::Success;
                break;
            }
        }
        if status == ParseResultStatus::AllChildrenFailed && all_didnt_match {
            status = ParseResultStatus::DidntMatch;
        }
        if let Some(closest_idx) = closest_to_match_idx {
            // let s = pr.status;
            for (idx, child) in children.iter_mut().enumerate() {
                if idx != closest_idx {
                    child.status = ParseResultStatus::Ignored;
                }
            }
            // pr.status = s;
        }
        return ParseResult {
            children: children,
            parent: node,
            generated: String::from(""),
            status: status,
            text: text.to_string(),
            parse_data: lua
                .create_table()
                .expect("Failed to create arg table for selector"),
        };
    }

    fn get_script(&self) -> String {
        SELECTOR_SCRIPT.to_string()
    }
}

#[cfg(test)]
mod tests {
    use regex::Regex;

    use super::*;

    #[test]
    fn selector_test_success() {
        let m1 = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("m1"),
            Regex::new("m1").unwrap(),
            String::from("function _Create(text, children, data) return 'matcher1' end"),
            vec![],
        )));
        let m2 = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("m2"),
            Regex::new("m2").unwrap(),
            String::from("function _Create(text, children, data) return 'matcher2' end"),
            vec![],
        )));
        let root = Rc::new(RefCell::new(ParserNode::selector(String::from("selector1"), vec![m1, m2])));

        let text = "m1";

        let lua = Lua::new();
        let parse_result = ParserNode::parse(root, text, &lua);
        assert_eq!(parse_result.status, ParseResultStatus::Success);
        assert_eq!(parse_result.generated, "matcher1");
    }

    #[test]
    fn selector_test_didnt_match() {
        let m1 = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("m1"),
            Regex::new("m1").unwrap(),
            String::from("function _Create(text, children, data) return 'matcher1' end"),
            vec![],
        )));
        let m2 = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("m2"),
            Regex::new("m2").unwrap(),
            String::from("function _Create(text, children, data) return 'matcher2' end"),
            vec![],
        )));
        let root = Rc::new(RefCell::new(ParserNode::selector(String::from("selector1"), vec![m1, m2])));

        let text = "m3";

        let lua = Lua::new();
        let parse_result = ParserNode::parse(root, text, &lua);
        assert_eq!(parse_result.status, ParseResultStatus::DidntMatch);
    }
}
