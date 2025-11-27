use crate::traits::*;
use sql_query_builder as sql;
use std::error::Error;

#[derive(Debug)]
pub struct CardModel {
    pub id: i32,
    pub name: String,
    pub text: String,
    pub project_name: String,
}

impl SQLCreate for CardModel {
    fn sql_create() -> sql::CreateTable {
        sql::CreateTable::new()
            .create_table_if_not_exists("cards")
            .column("id INTEGER")
            .column("name TEXT NOT NULL")
            .column("text TEXT NOT NULL")
            .column("project_name TEXT NOT NULL")
            .foreign_key("(project_name) REFERENCES projects(name)")
            .primary_key("id")
            .column("UNIQUE(name, project_name)")
    }
}

impl SQLDrop for CardModel {
    fn sql_drop() -> sql::DropTable {
        sql::DropTable::new().drop_table_if_exists("cards")
    }
}

impl SQLSelect for CardModel {
    fn sql_select() -> sql::Select {
        sql::Select::new().select("*").from("cards")
    }
}

// impl SQLInsert for CardModel {
//     fn sql_insert(&self) -> sql::Insert {
//         let values = format!(
//             "('{}', '{}', '{}')",
//             self.name, self.text, self.project_name
//         );
//         sql::Insert::new()
//             .insert_into("cards (name, text, project_name)")
//             .values(values.as_str())
//     }
// }


impl SQLInsertInto for CardModel {
    fn sql_insert_into(&self, conn: &rusqlite::Connection) -> Result<usize, Box<dyn Error>> {
        let sql = sql::Insert::new()
            .insert_into("cards (name, text, project_name)")
            .values("(?1, ?2, ?3)")
            .as_string();

        let result = conn.execute(&sql, (&self.name, &self.text, &self.project_name))?;
        Ok(result)
    }
}