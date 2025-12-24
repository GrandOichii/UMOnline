use core::panic;
use std::{error::Error, vec};

use crate::{
    parsers::{matcher::Matcher, parser::ParserNode, selector::Selector, splitter::Splitter},
    traits::*,
};
use regex::Regex;
use sql_query_builder as sql;

pub type ParserModelType = i32;
pub const PMT_MATCHER: ParserModelType = 1;
pub const PMT_SELECTOR: ParserModelType = 2;
pub const PMT_SPLITTER: ParserModelType = 3;

pub fn pmt_to_string(pmt: ParserModelType) -> String {
    match pmt {
        PMT_MATCHER => String::from("Matcher"),
        PMT_SELECTOR => String::from("Selector"),
        PMT_SPLITTER => String::from("Splitter"),
        _ => panic!("Unrecognized parser model type: {}", pmt),
    }
}

pub fn pmt_has_pattern(pmt: ParserModelType) -> bool {
    match pmt {
        PMT_MATCHER => true,
        PMT_SELECTOR => false,
        PMT_SPLITTER => true,
        _ => panic!("Unrecognized parser model type: {}", pmt),
    }
}

#[derive(Clone)]
pub struct ParserModel {
    pub name: String,
    pub ptype: ParserModelType,
    pub pattern: String,
    pub script: String,

    pub id: i32,
    pub project_name: String,
    pub description: String,
    pub is_template: bool,
    pub parent_id: Option<i32>,
    pub parent_slot: Option<i32>,
    pub ref_to_id: Option<i32>,
    pub ref_name: Option<String>,
    pub parser_editor_id: i32,

    pub editor_offset_x: f32,
    pub editor_offset_y: f32,

    pub children: Vec<ParserModel>,
}

impl SQLModel for ParserModel {
    fn sql_create() -> sql_query_builder::CreateTable {
        sql::CreateTable::new()
            .create_table_if_not_exists("parsers")
            .column("name TEXT NOT NULL")
            .column("ptype INTEGER NOT NULL")
            .column("pattern TEXT NOT NULL")
            .column("script TEXT NOT NULL")
            .column("id INTEGER")
            .column("project_name TEXT NOT NULL")
            .column("description TEXT NOT NULL")
            .column("is_template INTEGER NOT NULL")
            .column("parent_id INTEGER")
            .column("parent_slot INTEGER")
            .column("ref_to_id INTEGER")
            .column("editor_offset_x REAL NOT NULL")
            .column("editor_offset_y REAL NOT NULL")
            .column("parser_editor_id INTEGER NOT NULL")
            .primary_key("id")
            .foreign_key("(project_name) REFERENCES projects(name) ON DELETE CASCADE")
            .foreign_key("(parent_id) REFERENCES parsers(id) ON DELETE CASCADE")
            .foreign_key("(ref_to_id) REFERENCES parsers(id) ON DELETE CASCADE")
    }

    fn sql_drop() -> sql_query_builder::DropTable {
        sql::DropTable::new().drop_table_if_exists("parsers")
    }

    fn sql_insert_into(&self, conn: &rusqlite::Connection) -> Result<usize, Box<dyn Error>> {
        let sql = sql::Insert::new()
            .insert_into("parsers (name, ptype, pattern, script, project_name, description, is_template, parent_id, parent_slot, ref_to_id, editor_offset_x, editor_offset_y, parser_editor_id)")
            .values("(?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8, ?9, ?10, ?11, ?12, ?13)")
            .as_string();

        let result = conn.execute(
            &sql,
            (
                &self.name,
                &self.ptype,
                &self.pattern,
                &self.script,
                &self.project_name,
                &self.description,
                &self.is_template,
                &self.parent_id,
                &self.parent_slot,
                &self.ref_to_id,
                self.editor_offset_x,
                self.editor_offset_y,
                self.parser_editor_id,
            ),
        )?;
        Ok(result)
    }

    fn sql_delete() -> sql_query_builder::Delete {
        sql::Delete::new().delete_from("parsers")
    }

    fn sql_select() -> sql::Select {
        sql::Select::new()
            .select("p.*, refs.name")
            .from("parsers p")
            .left_join("parsers refs on p.ref_to_id = refs.id")
    }

    fn get_fn_mut(row: &rusqlite::Row) -> Result<Self, rusqlite::Error>
    where
        Self: Sized,
    {
        Ok(ParserModel {
            name: row.get(0)?,
            ptype: row.get(1)?,
            pattern: row.get(2)?,
            script: row.get(3)?,
            id: row.get(4)?,
            project_name: row.get(5)?,
            description: row.get(6)?,
            is_template: row.get(7)?,
            parent_id: row.get(8)?,
            parent_slot: row.get(9)?,
            ref_to_id: row.get(10)?,
            editor_offset_x: row.get(11)?,
            editor_offset_y: row.get(12)?,
            parser_editor_id: row.get(13)?,
            // reference
            ref_name: row.get(14)?,
            children: vec![],
        })
    }
}

impl SQLUpdateById for ParserModel {
    fn sql_update_by_id(&self, conn: &rusqlite::Connection) -> Result<usize, Box<dyn Error>> {
        let sql = sql::Update::new()
            .update("parsers")
            .set("name = ?2")
            .set("ptype = ?3")
            .set("pattern = ?4")
            .set("script = ?5")
            .set("project_name = ?6")
            .set("description = ?7")
            .set("is_template = ?8")
            .set("parent_id = ?9")
            .set("parent_slot = ?10")
            .set("ref_to_id = ?11")
            .set("editor_offset_x = ?12")
            .set("editor_offset_y = ?13")
            .set("parser_editor_id = ?14")
            .where_clause("id = ?1")
            .to_string();

        let result = conn.execute(
            &sql,
            (
                self.id,
                &self.name,
                &self.ptype,
                &self.pattern,
                &self.script,
                &self.project_name,
                &self.description,
                &self.is_template,
                &self.parent_id,
                &self.parent_slot,
                &self.ref_to_id,
                self.editor_offset_x,
                self.editor_offset_y,
                self.parser_editor_id,
            ),
        )?;

        return Ok(result);
    }
}

impl ParserModel {
    fn get_matcher_pattern(&self) -> String {
        format!("^{}$", self.pattern)
    }

    pub fn new_ref(ref_parser: &ParserModel) -> ParserModel {
        ParserModel {
            name: String::from("REF_NAME"),
            children: vec![],
            description: String::from(""),
            editor_offset_x: 0.0,
            editor_offset_y: 0.0,
            id: -1,
            is_template: false,
            parent_id: None,
            parent_slot: None,
            pattern: String::from(""),
            project_name: ref_parser.project_name.to_string(),
            ptype: ref_parser.ptype,
            ref_name: Some(ref_parser.name.to_string()),
            ref_to_id: Some(ref_parser.id),
            script: String::from(""),
            parser_editor_id: -1,
        }
    }

    pub fn to_parser_node<'a>(&self) -> Result<ParserNode, Box<dyn Error>> {
        Ok(ParserNode {
            name: self.name.to_string(),
            children: vec![],
            parser: match self.ptype {
                PMT_MATCHER => self.to_matcher()?,
                PMT_SELECTOR => self.to_selector()?,
                PMT_SPLITTER => self.to_splitter()?,
                _ => panic!("Unrecognized PMT: {}", self.ptype),
            },
        })
    }

    fn to_matcher(&self) -> Result<Box<Matcher>, Box<dyn Error>> {
        Ok(Box::new(Matcher {
            pattern: Regex::new(&self.get_matcher_pattern())?,
            script: self.script.to_string(),
        }))
    }

    fn to_selector(&self) -> Result<Box<Selector>, Box<dyn Error>> {
        Ok(Box::new(Selector))
    }

    fn to_splitter(&self) -> Result<Box<Splitter>, Box<dyn Error>> {
        Ok(Box::new(Splitter {
            pattern: Regex::new(&self.pattern)?,
            script: self.script.to_string(),
        }))
    }
}
