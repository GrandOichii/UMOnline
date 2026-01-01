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
        let matches = self.pattern.captures(text);
        let mut didnt_match = 0;
        if matches.is_none() {
            return ParseResult {
                status: ParseResultStatus::DidntMatch,
                generated: String::from(""),
                text: text.to_string(),
                parent: node,
                children: vec![],
                parse_data: lua
                    .create_table()
                    .expect("Failed to create arg table for matcher"),
            };
        }
        let matches = matches.unwrap();
        // println!("{:?}", matches.as_ref().unwrap());
        // println!("{}", matches.as_ref().unwrap().len());

        let table = lua
            .create_table()
            .expect("Failed to create arg table for matcher");

        for (i, m) in matches.iter().enumerate() {
            // println!("{:?}", m);
            if i == 0 {
                continue;
            }
            table
                .set(
                    i,
                    match m {
                        Some(s) => s.as_str(),
                        None => "",
                    }
                    .to_string(),
                )
                // .set(i + 1, group.)
                .expect("Failed to set arg table for matcher");
        }

        let mut result = ParseResult {
            status: ParseResultStatus::Success,
            text: text.to_string(),
            generated: String::from(""),
            parent: node.clone(),
            children: Vec::new(),
            parse_data: table,
        };
        if result.parent.borrow().children.len() == 0 {
            return result;
        }

        let n = node.borrow();
        for (i, m) in matches.iter().enumerate() {
            if i == 0 {
                continue;
            }
            let child = n.children[i - 1].clone();
            let child_result = ParserNode::parse(
                child,
                match m {
                    Some(s) => s.as_str(),
                    None => "",
                },
                lua,
            );
            if child_result.status != ParseResultStatus::Success {
                result.status = ParseResultStatus::ChildFailed;
            }
            if child_result.status == ParseResultStatus::DidntMatch {
                didnt_match += 1;
            }
            result.children.push(child_result);
        }
        // if didnt_match == n.children.len() {
        //     result.status = ParseResultStatus::DidntMatch;
        // }
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
        assert_eq!(parse_result.generated, "something");
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

    #[test]
    fn generic_test1() {
        let root = Rc::new(RefCell::new(ParserNode::matcher(
            String::from("root"),
            Regex::new("^If (.+), (.+)$").unwrap(),
            String::from("function _Create(text, children, data) return children[1]..' '..children[2] end"),
            // String::from("function _Create(text, children, data) return children[1] end"),
            vec![
                Rc::new(RefCell::new(ParserNode::matcher(
                    String::from("c1"),
                    Regex::new(".+").unwrap(),
                    String::from("function _Create(text, children, data) return 'c1'..text end"),
                    vec![]
                ))),
                Rc::new(RefCell::new(ParserNode::matcher(
                    String::from("c2"),
                    Regex::new(".+").unwrap(),
                    String::from("function _Create(text, children, data) return 'c2'..text end"),
                    vec![]
                ))),
            ]
        )));

        let text = "If A, B";

        let lua = Lua::new();
        let parse_result = ParserNode::parse(root, text, &lua);
        assert_eq!(parse_result.status, ParseResultStatus::Success);
        assert_eq!(parse_result.generated, "c1A c2B");
    }
}
