use std::{cell::RefCell, error::Error, rc::Rc};

use mlua::Lua;

#[derive(Debug, PartialEq, Clone, Copy)]
pub enum ParseResultStatus {
    Success,
    DidntMatch,
    ChildFailed,
    AllChildrenFailed,
    Ignored,
}

pub struct ParseResult {
    pub status: ParseResultStatus,
    pub text: String,
    pub generated: String,
    pub parent: Rc<RefCell<ParserNode>>,
    pub children: Vec<ParseResult>,
    pub parse_data: mlua::Table,
}

impl ParseResult {
    pub fn create_script(&self, lua: &Lua) -> Result<String, Box<dyn Error>> {
        if self.status != ParseResultStatus::Success {
            return Ok(String::from(""));
        }
        let children_table = lua.create_table().expect("Failed to create table");

        for (i, child) in self.children.iter().enumerate() {
            let child_script = child.create_script(lua)?;
            children_table
                .set(i + 1, child_script)
                .expect("Failed to add child script for childen_table");
        }

        lua.load(self.parent.borrow().parser.get_script()).exec()?;
        let creation_func: mlua::Function = lua.globals().get("_Create")?;

        let result = creation_func.call::<String>((
            self.text.to_string(),
            &children_table,
            &self.parse_data,
        ))?;
        return Ok(result);
    }
}

pub trait Parser {
    fn parse(&self, text: &str, node: Rc<RefCell<ParserNode>>, lua: &Lua) -> ParseResult;

    fn get_script(&self) -> String;
}

pub struct ParserNode {
    pub name: String,
    pub parser: Box<dyn Parser>,
    pub children: Vec<Rc<RefCell<ParserNode>>>,
}

impl ParserNode {
    pub fn parse(this: Rc<RefCell<Self>>, text: &str, lua: &Lua) -> ParseResult {
        let parser = this.borrow();
        let mut result = parser.parser.parse(text, this.clone(), lua);
        let script = result.create_script(lua);
        result.generated = match script {
            Ok(s) => s,
            Err(err) => err.to_string(),
        };
        return result;
    }
}
