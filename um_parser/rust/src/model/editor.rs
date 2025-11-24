use crate::traits::*;
use sql_query_builder as sql;

pub struct EditorModel {
    pub last_project_name: String,
}

impl SQLDrop for EditorModel {
    fn sql_drop() -> sql::DropTable {
        sql::DropTable::new().drop_table("editors")
    }
}

impl SQLCreate for EditorModel {
    fn sql_create() -> sql::CreateTable {
        sql::CreateTable::new()
            .create_table_if_not_exists("editors")
            .column("id SERIAL")
            .column("last_project_name TEXT NOT NULL")
            .primary_key("id")
    }
}

impl SQLInsert for EditorModel {
    fn sql_insert(&self) -> sql::Insert {
        let values = format!("('{}')", self.last_project_name);
        sql::Insert::new()
            .insert_into("editors (last_project_name)")
            .values(values.as_str())
    }
}

impl SQLSelect for EditorModel {
    fn sql_select() -> sql_query_builder::Select {
        sql::Select::new()
            .select("last_project_name")
            .from("editors")
    }
}
