use regex::Regex;

use crate::parser::*;

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

impl Splitter {
    pub fn new<'a>(name: String, pattern: Regex, children: Vec<&'a ParserNode>) -> ParserNode<'a> {
        ParserNode {
            name: name,
            parser: Box::new(Splitter {
                script: SPLITTER_SCRIPT.to_string(),
                pattern: pattern,
            }),
            children: children,
        }   
    }

    pub fn new_with_script<'a>(name: String, pattern: Regex, script: String, children: Vec<&'a ParserNode>) -> ParserNode<'a> {
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
