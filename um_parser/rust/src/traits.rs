use sql_query_builder::Select;



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