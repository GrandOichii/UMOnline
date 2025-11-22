use sql_query_builder::{Delete, Select};

pub trait SQLCreate {
    fn sql_create() -> String;    
}

pub trait SQLDrop {
    fn sql_drop() -> String;
}

pub trait SQLSelect {
    fn sql_select() -> Select;
}

pub trait SQLInsert {
    fn sql_insert(&self) -> String;
}

pub trait SQLDelete {
    fn sql_delete() -> Delete;
}