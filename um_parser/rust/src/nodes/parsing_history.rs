use std::collections::HashMap;

use godot::classes::*;
use godot::prelude::*;

use crate::parsers::parser::{ParseResult, ParseResultStatus};

#[derive(Default)]
pub struct ParsingHistory {
    pub parse_result_map: HashMap<String, ParserParsingHistory>,
    pub parse_results: Vec<ParseResult>,
}

pub struct ParsedText {
    pub text: String,
    pub full_text: String,
}

pub struct ParserParsingHistory {
    pub parsed_texts: Vec<ParsedText>,
    pub unparsed_texts: Vec<ParsedText>,
}

impl ParserParsingHistory {
    pub fn process_parse_result(&mut self, root: &ParseResult, pr: &ParseResult) {
        match pr.status {
            ParseResultStatus::Success => &mut self.parsed_texts,
            _other => &mut self.unparsed_texts,
        }
        .push(ParsedText {
            full_text: root.text.to_string(),
            text: pr.text.to_string(),
        })
    }
}

impl ParsingHistory {
    pub fn get_for(&self, parser_name: &String) -> Option<&ParserParsingHistory> {
        self.parse_result_map.get(parser_name)
    }

    pub fn from_parse_results(parse_results: Vec<ParseResult>) -> ParsingHistory {
        let mut parse_result_map = HashMap::<String, ParserParsingHistory>::new();

        ParsingHistory::fill_parse_result_map(&mut parse_result_map, &parse_results, None);

        ParsingHistory {
            parse_result_map: parse_result_map,
            parse_results: parse_results,
        }
    }

    fn fill_parse_result_map(
        parse_result_map: &mut HashMap<String, ParserParsingHistory>,
        prs: &Vec<ParseResult>,
        root_pr: Option<&ParseResult>,
    ) {
        for pr in prs {
            let root = match root_pr {
                None => Some(pr),
                Some(root_p) => Some(root_p),
            };

            let name = pr.parent.borrow().name.to_string();
            match parse_result_map.get_mut(&name) {
                Some(pph) => {
                    pph.process_parse_result(pr, root.unwrap());
                }
                None => {
                    parse_result_map.insert(
                        name,
                        ParserParsingHistory {
                            parsed_texts: vec![],
                            unparsed_texts: vec![],
                        },
                    );
                }
            };
            ParsingHistory::fill_parse_result_map(parse_result_map, &pr.children, root);
        }
    }

    pub fn total_len(&self) -> usize {
        self.parse_results.len()
    }

    pub fn parsed_len(&self) -> usize {
        self.parse_results
            .iter()
            .filter(|pr| pr.status == ParseResultStatus::Success)
            .count()
    }
}

#[derive(GodotClass)]
#[class(init,base=Node)]
pub struct ParsingHistoryNode {
    base: Base<Node>,
    //#[export_group(name="Nodes")]
}

#[godot_api]
impl ParsingHistoryNode {}

#[godot_api]
impl INode for ParsingHistoryNode {
    fn ready(&mut self) {}
}

impl ParsingHistoryNode {
    pub fn add_history(&mut self, ph: ParsingHistory) {}
}
