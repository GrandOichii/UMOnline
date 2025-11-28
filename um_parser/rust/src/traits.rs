use std::error::Error;

use rusqlite::{Connection, Row};
use sql_query_builder as sql;

pub trait SQLModel {
    fn sql_create() -> sql::CreateTable;    

    fn sql_drop() -> sql::DropTable;

    fn sql_select() -> sql::Select;

    fn sql_delete() -> sql::Delete;

    fn sql_insert_into(&self, conn: &Connection) -> Result<usize, Box<dyn Error>>;

    fn get_fn_mut(row: &Row) -> Result<Self, rusqlite::Error> where Self: Sized;
}