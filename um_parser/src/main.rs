use std::fs;
use std::vec;

use regex::Regex;

mod matcher;
mod parser;
mod selector;
mod splitter;

use crate::matcher::*;
use crate::parser::*;
use crate::selector::*;
use crate::splitter::Splitter;

fn main() {
    let static_amount = Matcher::new(
        String::from("static_amount"),
        Regex::new("[0-9]").unwrap(),
        String::from("value"),
        vec![],

    );
    let amount_select = Selector::new(String::from("amount_select"), vec![&static_amount]);

    let draw = Matcher::new(
        String::from("root"),
        Regex::new("[D|d]raw (.+) cards?").unwrap(),
        String::from("function _Create(text, children, data) return 'TODO' end"),
        vec![&amount_select],
    );
    let root = Splitter::new(
        String::from("sentence_splitter"),
        Regex::new("\\. ").unwrap(),
        vec![&draw],
    );

    let texts = vec![
        "Draw 2 cards. Draw 1 card",
        // "draw 1 card",
        // "draw up to 4 cards"
    ];
    for text in texts {
        root.parse(text);
    }
}

fn read_script(from: &str) -> String {
    let result = fs::read_to_string(from).expect("Failed to read input");
    return result;
}
