use sql_query_builder::{self as sql, Select};
use crate::traits::*;

#[derive(Debug)]
pub struct ProjectModel {
    pub name: String,
    pub description: String,
}

impl SQLCreate for ProjectModel {
    fn sql_create() -> String {
        sql::CreateTable::new()
            .create_table_if_not_exists("projects")
            .column("name TEXT NOT NULL")
            .column("description TEXT NOT NULL")
            .primary_key("name")
            .as_string()
    }
}

impl SQLDrop for ProjectModel {
    fn sql_drop() -> String {
        sql::DropTable::new()
            .drop_table_if_exists("projects")
            .as_string()
    }
}

impl SQLSelect for ProjectModel {
    fn sql_select() -> Select {
        sql::Select::new()
            .select("*")
            .from("projects")
    }
}

impl SQLInsert for ProjectModel {
    fn sql_insert(&self) -> String {
        let values = format!("('{}', '{}')", self.name, self.description);
        sql::Insert::new()
            .insert_into("projects (name, description)")
            .values(values.as_str())
            .as_string()
    }
}