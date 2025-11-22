use sql_query_builder as sql;

pub trait SQLCreate {
    fn sql_create() -> sql::CreateTable;    
}

pub trait SQLDrop {
    fn sql_drop() -> sql::DropTable;
}

pub trait SQLSelect {
    fn sql_select() -> sql::Select;
}

pub trait SQLInsert {
    fn sql_insert(&self) -> sql::Insert;
}

pub trait SQLDelete {
    fn sql_delete() -> sql::Delete;
}