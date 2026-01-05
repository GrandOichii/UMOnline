use crate::traits::*;
use rusqlite::params;
use sql_query_builder as sql;

pub struct EditorModel {
    pub last_project_name: String,
}

impl SQLModel for EditorModel {
    fn sql_drop() -> sql::DropTable {
        sql::DropTable::new().drop_table_if_exists("editors")
    }

    fn sql_create() -> sql::CreateTable {
        sql::CreateTable::new()
            .create_table_if_not_exists("editors")
            .column("id SERIAL")
            .column("last_project_name TEXT NOT NULL")
            .primary_key("id")
    }

    fn sql_select() -> sql_query_builder::Select {
        sql::Select::new()
            .select("last_project_name")
            .from("editors")
    }

    fn sql_delete() -> sql_query_builder::Delete {
        sql::Delete::new().delete_from("editors")
    }

    fn sql_insert_into(
        &self,
        conn: &rusqlite::Connection,
    ) -> Result<usize, Box<dyn std::error::Error>> {
        let sql = sql::Insert::new()
            .insert_into("editors (last_project_name)")
            .values("(?1)")
            .to_string();

        let result = conn.execute(&sql, params!(&self.last_project_name))?;
        Ok(result)
    }

    fn get_fn_mut(row: &rusqlite::Row) -> Result<Self, rusqlite::Error>
    where
        Self: Sized,
    {
        Ok(EditorModel {
            last_project_name: row.get(0)?,
        })
    }
}
