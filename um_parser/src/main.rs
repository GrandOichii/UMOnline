use std::vec;

use regex::Regex;

mod parser;
mod matcher;
mod selector;
mod splitter;

use crate::parser::*;
use crate::matcher::*;
use crate::selector::*;
use crate::splitter::Splitter;

fn main() {
    let static_amount = ParserNode {
        name: String::from("static_amount"),
        parser: Box::new(Matcher {
            script: String::from("value"),
            pattern: Regex::new("[0-9]").unwrap(),
        }),
        children: vec![],
    };
    let amount_select = ParserNode {
        name: String::from("amount_select"),
        parser: Box::new(Selector),
        children: vec![&static_amount]
    };
    let draw = ParserNode {
        name: String::from("root"),
        parser: Box::new(Matcher {
            script: String::from("function _Create(text, children, data) return 'TODO' end"),
            pattern: Regex::new("[D|d]raw (.+) cards?").unwrap(),
        }),
        children: vec![&amount_select],
    };
    let root = ParserNode {
        name: String::from("sentence_splitter"),
        children: vec![&draw],
        parser: Box::new(Splitter {
            script: String::from("function _Create(text, children, data) return 'TODO' end"),
            pattern: Regex::new("\\. ").unwrap(),
        })
    };


    let texts = vec![
        "Draw 2 cards. Draw 1 card",
        // "draw 1 card",
        // "draw up to 4 cards"
    ];
    for text in texts {
        root.parse(text);
    }
}
