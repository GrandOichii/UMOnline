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
    pub parent: &'a dyn Parser,
    pub children: Vec<ParseResult<'a>>,
}

pub trait Parser {
    fn parse<'a>(&'a self, text: &str, node: &ParserNode<'a>) -> ParseResult<'a>;
}


pub struct ParserNode<'a> {
    pub name: String,
    pub parser: Box<dyn Parser>,
    pub children: Vec<&'a ParserNode<'a>>,
}

impl<'a> ParserNode<'a> {
    pub fn parse(&'_ self, text: &str) -> ParseResult<'_> {
        let result = self.parser.parse(text, self);
        println!("{} status: {:?}", self.name, result.status);
        return result;
    }
}