use regex::Regex;

use crate::parser::*;

pub struct Splitter {
    pub script: String,
    pub pattern: Regex,
}

impl Parser for Splitter {
    fn parse<'a>(&'a self, text: &str, node: &ParserNode<'a>) -> ParseResult<'a> {
        println!("Splitting {text}");
        let split = self.pattern.split(text);
        let mut status = ParseResultStatus::Success;
        let mut children: Vec<ParseResult> = Vec::new();
        let child = node.children[0];

        let mut split_count: usize = 0;
        let mut failed = 0;

        for part in split {
            if part.is_empty() {
                continue;
            }
            let part_result = child.parse(part);
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
            parent: self,
            children: children,
        };
    }
}
