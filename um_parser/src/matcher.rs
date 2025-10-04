use regex::Regex;

use crate::parser::*;

pub struct Matcher {
    pub script: String,
    pub pattern: Regex,
}

impl Matcher {
    pub fn new<'a>(name: String, pattern: Regex, script: String, children: Vec<&'a ParserNode>) -> ParserNode<'a> {
        ParserNode {
            name: name,
            children: children,
            parser: Box::new(Matcher {
                pattern: pattern,
                script: script,
            })
        }
    }
}

impl Parser for Matcher {
    fn parse<'a>(&'a self, text: &str, node: &ParserNode<'a>) -> ParseResult<'a> {
        println!("Matching {}", text);
        let m = self.pattern.captures(text);
        let mut didnt_match = -1;
        let mut result = ParseResult {
            status: match m {
                None => ParseResultStatus::DidntMatch,
                _ => ParseResultStatus::Success,
            },
            text: text.to_string(),
            parent: self,
            children: Vec::new(),
        };
        let mut i = 1;
        if node.children.len() == 0 {
            return result;
        }
        for g in m.iter() {
            let child = node.children[i - 1];
            let child_result = child.parse(g.get(1).unwrap().as_str());
            i += 1;
            if child_result.status != ParseResultStatus::Success {
                result.status = ParseResultStatus::ChildFailed;
            }
            if child_result.status == ParseResultStatus::DidntMatch {
                didnt_match = match didnt_match {
                    -1 => 1,
                    x => x + 1,
                };
            }
            result.children.push(child_result);
        }
        return result;
    }
}
