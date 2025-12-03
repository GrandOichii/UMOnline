use std::collections::HashMap;

use godot::classes::*;
use godot::prelude::*;

use crate::parsers::parser::{ParseResult, ParseResultStatus};

#[derive(Default)]
pub struct ParsingHistory {
    pub parse_result_map: HashMap<String, ParserParsingHistory>,
    pub parse_results: Vec<ParseResult>,
}

pub struct ParserParsingHistory {
    pub parsed_texts: Vec<String>,
    pub unparsed_texts: Vec<String>,
}

impl ParserParsingHistory {
    pub fn process_parse_result(&mut self, pr: &ParseResult) {
        match pr.status {
            ParseResultStatus::Success => {
                self.parsed_texts.push(pr.text.to_string());
            }
            _other => {
                self.unparsed_texts.push(pr.text.to_string());
            }
        }
    }
}

impl ParsingHistory {
    pub fn get_for(&self, parser_name: &String) -> Option<&ParserParsingHistory> {
        self.parse_result_map.get(parser_name)
    }

    pub fn from_parse_results(parse_results: Vec<ParseResult>) -> ParsingHistory {
        let mut parse_result_map = HashMap::<String, ParserParsingHistory>::new();

        ParsingHistory::fill_name_me(&mut parse_result_map, &parse_results);

        ParsingHistory {
            parse_result_map: parse_result_map,
            parse_results: parse_results,
        }
    }

    fn fill_name_me(
        parse_result_map: &mut HashMap<String, ParserParsingHistory>,
        prs: &Vec<ParseResult>,
    ) {
        for pr in prs {
            let name = pr.parent.borrow().name.to_string();
            match parse_result_map.get_mut(&name) {
                Some(pph) => {
                    pph.process_parse_result(pr);
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
            ParsingHistory::fill_name_me(parse_result_map, &pr.children);
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
