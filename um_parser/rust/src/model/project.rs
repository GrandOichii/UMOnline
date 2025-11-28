use std::error::Error;

use crate::traits::*;
use rusqlite::Row;
use sql_query_builder::{self as sql, Select};

#[derive(Debug)]
pub struct ProjectModel {
    pub name: String,
    pub description: String,
}

impl SQLModel for ProjectModel {
    fn sql_create() -> sql::CreateTable {
        sql::CreateTable::new()
            .create_table_if_not_exists("projects")
            .column("name TEXT NOT NULL")
            .column("description TEXT NOT NULL")
            .primary_key("name")
    }

    fn sql_drop() -> sql::DropTable {
        sql::DropTable::new().drop_table_if_exists("projects")
    }

    fn sql_select() -> Select {
        sql::Select::new().select("*").from("projects")
    }

    fn sql_delete() -> sql::Delete {
        sql::Delete::new().delete_from("projects")
    }

    fn get_fn_mut(row: &Row) -> Result<Self, rusqlite::Error> {
        Ok(ProjectModel {
            name: row.get(0)?,
            description: row.get(1)?,
        })
    }
    
    fn sql_insert_into(&self, conn: &rusqlite::Connection) -> Result<usize, Box<dyn Error>> {
        let sql = sql::Insert::new()
            .insert_into("projects (name, description)")
            .values("(?1, ?2)")
            .to_string();

        let result = conn.execute(&sql, (&self.name, &self.description))?;
        Ok(result)
    }
}

impl ProjectModel {
    pub fn sql_update_description(new_description: &String) -> sql::Update {
        sql::Update::new()
            .update("projects")
            .set(format!("description = '{}'", new_description).as_str())
    }
}