use std::collections::HashMap;

use godot::prelude::*;

use crate::parsers::parser::{ParseResult, ParseResultStatus};

#[derive(Default)]
pub struct ParsingHistory {
    pub parse_result_map: HashMap<String, ParserParsingHistory>,
    pub parse_results: Vec<ParseResult>,
    pub card_scripts: HashMap<i32, String>,
}

pub struct ParsedText {
    pub original: String,
    pub full_text: String,
    pub generated: String,
}

pub struct ParserParsingHistory {
    pub parsed_texts: Vec<ParsedText>,
    pub unparsed_texts: Vec<ParsedText>,
}

impl ParserParsingHistory {
    pub fn process_parse_result(&mut self, pr: &ParseResult, root: &ParseResult) {
        match pr.status {
            ParseResultStatus::Success => &mut self.parsed_texts,
            ParseResultStatus::ChildFailed => &mut self.unparsed_texts,
            ParseResultStatus::AllChildrenFailed => &mut self.unparsed_texts,
            _ => return,
        }
        .push(ParsedText {
            full_text: root.text.to_string(),
            original: pr.text.to_string(),
            generated: pr.generated.to_string(),
        })
    }
}

impl ParsingHistory {
    pub fn get_for(&self, parser_name: &String) -> Option<&ParserParsingHistory> {
        self.parse_result_map.get(parser_name)
    }

    pub fn new(
        card_scripts: HashMap<i32, String>,
        parse_results: Vec<ParseResult>,
    ) -> ParsingHistory {
        let mut parse_result_map = HashMap::<String, ParserParsingHistory>::new();

        ParsingHistory::fill_parse_result_map(&mut parse_result_map, &parse_results, None);

        ParsingHistory {
            parse_result_map: parse_result_map,
            parse_results: parse_results,
            card_scripts: card_scripts,
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
            let pph = parse_result_map
                .entry(name)
                .or_insert(ParserParsingHistory {
                    parsed_texts: vec![],
                    unparsed_texts: vec![],
                });
            pph.process_parse_result(pr, root.unwrap());
            match pr.status {
                ParseResultStatus::Ignored | ParseResultStatus::DidntMatch => continue,
                _ => ParsingHistory::fill_parse_result_map(parse_result_map, &pr.children, root)
            }
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

    pub fn get_script_for(&self, id: i32) -> Option<String> {
        match self.card_scripts.get(&id) {
            Some(script) => Some(script.to_string()),
            None => None,
        }
    }
}
