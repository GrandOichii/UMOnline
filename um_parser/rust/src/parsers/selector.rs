use mlua::Lua;

use crate::parsers::parser::*;

pub struct Selector;

const SELECTOR_SCRIPT: &'static str = "function _Create(text, children)
    for _, child in ipairs(children) do
        if child ~= '' then
            return child
        end
    end
end";

impl ParserNode<'_> {
    pub fn selector<'a>(name: String, children: Vec<&'a ParserNode>) -> ParserNode<'a> {
        ParserNode {
            name: name,
            parser: Box::new(Selector),
            children: children,
        }
    }
}

impl Parser for Selector {
    fn parse<'a>(&'a self, text: &str, node: &'a ParserNode<'a>, lua: &Lua) -> ParseResult<'a> {
        println!("Selecting {}", text);
        let mut children: Vec<ParseResult> = Vec::new();
        let mut closest_to_match_idx: Option<usize> = None;
        let mut all_didnt_match = true;
        let mut status = ParseResultStatus::AllChildrenFailed;
        for child in node.children.iter() {
            children.push(child.parse(text, lua));
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
            status: status,
            text: text.to_string(),
            parse_data: lua.create_table().expect("Failed to create arg table for selector")
        };
    }

    fn get_script(&self) -> String {
        SELECTOR_SCRIPT.to_string()
    }
}
