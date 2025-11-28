use std::error::Error;

use mlua::Lua;

#[derive(Debug)]
#[derive(PartialEq)]
#[derive(Clone, Copy)]
pub enum ParseResultStatus {
    Success,
    DidntMatch,
    ChildFailed,
    AllChildrenFailed,
    Ignored,
}

pub struct ParseResult<'a> {
    pub status: ParseResultStatus,
    pub text: String,
    pub parent: &'a ParserNode<'a>,
    pub children: Vec<ParseResult<'a>>,
    pub parse_data: mlua::Table,
}

impl ParseResult<'_> {
    pub fn create_script(&self, lua: &Lua) -> Result<String, Box<dyn Error>> {
        if self.status != ParseResultStatus::Success {
            return Ok(String::from(""));
        }
        let children_table = lua.create_table()
            .expect("Failed to create table");

        for (i, child) in self.children.iter().enumerate() {
            let child_script = child.create_script(lua)?;
            children_table.set(i+1, child_script)
                .expect("Failed to add child script for childen_table");
        }

        lua.load(self.parent.parser.get_script()).exec()?;
        let creation_func: mlua::Function = lua.globals().get("_Create")?;
        
        let result = creation_func.call::<String>((self.text.to_string(), &children_table, &self.parse_data))?;
        return Ok(result);
    }
}

pub trait Parser {
    fn parse<'a>(&'a self, text: &str, node: &'a ParserNode<'a>, lua: &Lua) -> ParseResult<'a>;

    fn get_script(&self) -> String;
}


pub struct ParserNode<'a> {
    pub name: String,
    pub parser: Box<dyn Parser>,
    pub children: Vec<&'a ParserNode<'a>>,
}

impl<'a> ParserNode<'a> {
    pub fn parse(&'_ self, text: &str, lua: &Lua) -> ParseResult<'_> {
        let result = self.parser.parse(text, self, lua);
        return result;
    }
}