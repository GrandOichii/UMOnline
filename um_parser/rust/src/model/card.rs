use sql_query_builder::{self as sql, Select};
use crate::traits::{SQLCreate, SQLDrop, SQLInsert, SQLSelect};

#[derive(Debug)]
pub struct CardModel {
    pub name: String,
    pub text: String,
    pub project_name: String,
}

impl SQLCreate for CardModel {
    fn sql_create() -> String {
        sql::CreateTable::new()
            .create_table_if_not_exists("cards")
            .column("name TEXT NOT NULL")
            .column("text TEXT NOT NULL")
            .column("project_name TEXT NOT NULL")
            .foreign_key("(project_name) REFERENCES projects(name)")
            .primary_key("name")
            .as_string()
    }
}


impl SQLDrop for CardModel {
    fn sql_drop() -> String {
        sql::DropTable::new()
            .drop_table_if_exists("cards")
            .as_string()
    }
}

impl SQLSelect for CardModel {
    fn sql_select() -> Select {
        sql::Select::new()
            .select("*")
            .from("cards")
    }
}

impl SQLInsert for CardModel {
    fn sql_insert(&self) -> String {
        let values = format!("('{}', '{}', '{}')", self.name, self.text, self.project_name);
        sql::Insert::new()
            .insert_into("cards (name, text, project_name)")
            .values(values.as_str())
            .as_string()
    }
}