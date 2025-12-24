use crate::traits::*;
use rusqlite::Row;
use sql_query_builder as sql;
use std::error::Error;

#[derive(Debug)]
pub struct CardModel {
    pub id: i32,
    pub name: String,
    pub text: String,
    pub project_name: String,
}

impl SQLModel for CardModel {
    fn sql_create() -> sql::CreateTable {
        sql::CreateTable::new()
            .create_table_if_not_exists("cards")
            .column("id INTEGER")
            .column("name TEXT NOT NULL")
            .column("text TEXT NOT NULL")
            .column("project_name TEXT NOT NULL")
            .foreign_key("(project_name) REFERENCES projects(name) ON DELETE CASCADE")
            .primary_key("id")
            .column("UNIQUE(name, project_name)")
    }

    fn sql_drop() -> sql::DropTable {
        sql::DropTable::new().drop_table_if_exists("cards")
    }

    fn sql_select() -> sql::Select {
        sql::Select::new().select("*").from("cards")
        // .limit("1")
    }

    fn sql_insert_into(&self, conn: &rusqlite::Connection) -> Result<usize, Box<dyn Error>> {
        let sql = sql::Insert::new()
            .insert_into("cards (name, text, project_name)")
            .values("(?1, ?2, ?3)")
            .as_string();

        let result = conn.execute(&sql, (&self.name, &self.text, &self.project_name))?;
        Ok(result)
    }

    fn get_fn_mut(row: &Row) -> Result<Self, rusqlite::Error> {
        Ok(CardModel {
            id: row.get(0)?,
            name: row.get(1)?,
            text: row.get(2)?,
            project_name: row.get(3)?,
        })
    }

    fn sql_delete() -> sql_query_builder::Delete {
        sql::Delete::new().delete_from("cards")
    }
}
