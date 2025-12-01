use std::error::Error;

use crate::traits::*;
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
    pub is_root: bool,
    pub parent_id: Option<i32>,
    pub is_ref: bool,

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
            .column("is_root INTEGER NOT NULL")
            .column("parent_id INTEGER")
            .column("is_ref INTEGER NOT NULL")
            .column("editor_offset_x REAL NOT NULL")
            .column("editor_offset_y REAL NOT NULL")
            .primary_key("id")
            .foreign_key("(project_name) REFERENCES projects(name) ON DELETE CASCADE")
            .foreign_key("(parent_id) REFERENCES parsers(id) ON DELETE CASCADE")
    }

    fn sql_drop() -> sql_query_builder::DropTable {
        sql::DropTable::new().drop_table_if_exists("parsers")
    }

    fn sql_select() -> sql::Select {
        sql::Select::new().select("*").from("parsers")
    }

    fn sql_insert_into(&self, conn: &rusqlite::Connection) -> Result<usize, Box<dyn Error>> {
        let sql = sql::Insert::new()
            .insert_into("parsers (name, ptype, pattern, script, project_name, description, is_template, is_root, parent_id, is_ref, editor_offset_x, editor_offset_y)")
            .values("(?1, ?2, ?3, ?4, ?5, ?6, ?7, ?8, ?9, ?10, ?11, ?12)")
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
                &self.is_root,
                &self.parent_id,
                &self.is_ref,
                self.editor_offset_x,
                self.editor_offset_y,
            ),
        )?;
        Ok(result)
    }

    fn sql_delete() -> sql_query_builder::Delete {
        sql::Delete::new().delete_from("parsers")
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
            is_root: row.get(8)?,
            parent_id: row.get(9)?,
            is_ref: row.get(10)?,
            editor_offset_x: row.get(11)?,
            editor_offset_y: row.get(12)?,
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
            .set("is_root = ?9")
            .set("parent_id = ?10")
            .set("is_ref = ?11")
            .set("editor_offset_x = ?12")
            .set("editor_offset_y = ?13")
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
                &self.is_root,
                &self.parent_id,
                &self.is_ref,
                self.editor_offset_x,
                self.editor_offset_y,
            ),
        )?;        

        return Ok(result);
    }
}
