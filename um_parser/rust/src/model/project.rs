use crate::traits::*;
use sql_query_builder::{self as sql, Select};

#[derive(Debug)]
pub struct ProjectModel {
    pub name: String,
    pub description: String,
}

impl SQLCreate for ProjectModel {
    fn sql_create() -> sql::CreateTable {
        sql::CreateTable::new()
            .create_table_if_not_exists("projects")
            .column("name TEXT NOT NULL")
            .column("description TEXT NOT NULL")
            .primary_key("name")
    }
}

impl SQLDrop for ProjectModel {
    fn sql_drop() -> sql::DropTable {
        sql::DropTable::new().drop_table_if_exists("projects")
    }
}

impl SQLSelect for ProjectModel {
    fn sql_select() -> Select {
        sql::Select::new().select("*").from("projects")
    }
}

impl SQLInsert for ProjectModel {
    fn sql_insert(&self) -> sql::Insert {
        let values = format!("('{}', '{}')", self.name, self.description);
        sql::Insert::new()
            .insert_into("projects (name, description)")
            .values(values.as_str())
    }
}

impl SQLDelete for ProjectModel {
    fn sql_delete() -> sql::Delete {
        sql::Delete::new().delete_from("projects")
    }
}

impl ProjectModel {
    pub fn sql_update_description(new_description: &String) -> sql::Update {
        sql::Update::new()
            .update("projects")
            .set(format!("description = '{}'", new_description).as_str())
    }
}