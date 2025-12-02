use godot::prelude::*;
use regex::Regex;
use godot::classes::*;

pub mod parsers;
pub mod nodes;
pub mod model;
pub mod traits;
pub mod repo;

use crate::parsers::parser::*;

struct UMParserExtension;

#[gdextension]
unsafe impl ExtensionLibrary for UMParserExtension {}


#[derive(GodotClass)]
#[class(init,base=Control)]
struct TestNode {
    base: Base<Control>,

    #[export]
    label: OnEditor<Gd<Label>>,
}

#[godot_api]
impl IControl for TestNode {
    fn ready(&mut self) {
        self.label.set_text("Hello peter");
    }
}

// pub fn foo() {
//     let static_amount = ParserNode::matcher(
//         String::from("static_amount"),
//         Regex::new("[0-9]").unwrap(),
//         String::from("value"),
//         vec![],
//     );
//     let amount_select = ParserNode::selector(String::from("amount_select"), vec![&static_amount]);

//     let draw = ParserNode::matcher(
//         String::from("root"),
//         Regex::new("[D|d]raw (.+) cards?").unwrap(),
//         String::from("function _Create(text, children, data) return 'TODO' end"),
//         vec![&amount_select],
//     );
//     let root = ParserNode::splitter(
//         String::from("sentence_splitter"),
//         Regex::new("\\. ").unwrap(),
//         vec![&draw],
//     );

//     let texts = vec![
//         "Draw 2 cards. Draw 1 card",
//         // "draw 1 card",
//         // "draw up to 4 cards"
//     ];

//     // for text in texts {
//     //     let result = root.parse(text);
//     //     let _script = result.create_script()
//     //         .expect("Failed to create script");

//     // }
// }

// pub fn add(left: u64, right: u64) -> u64 {
//     left + right
// }

// #[cfg(test)]
// mod tests {
//     use super::*;

//     // #[test]
//     // fn it_works() {
//     //     let result = add(2, 2);
//     //     assert_eq!(result, 4);
//     // }
// }
