use rusqlite::Params;
use sql_query_builder as sql;
use sql_query_builder::Select;
use std::cell::OnceCell;
use std::error::Error;

use godot::classes::*;
use godot::prelude::*;
use rusqlite::Connection;
use rusqlite::params;

use crate::model::card::CardModel;
use crate::model::editor::EditorModel;
use crate::model::parser::ParserModel;
use crate::model::project::ProjectModel;
use crate::traits::*;

#[derive(GodotClass)]
#[class(init,base=Node)]
pub struct ParserRepositoryNode {
    base: Base<Node>,

    #[export]
    drop_tables_on_launch: bool,

    #[export]
    file_path: GString,

    connection: OnceCell<Connection>,
}

#[godot_api]
impl ParserRepositoryNode {
    #[signal]
    pub fn parser_updated(parser_id: u32);
}

impl ParserRepositoryNode {
    fn query<T: SQLModel>(
        &mut self,
        sql: sql::Select,
        params: impl Params,
    ) -> Result<Vec<T>, Box<dyn Error>> {
        let sql = sql.as_string();
        let mut stmt = self.get_connection().prepare(&sql)?;

        let rows = stmt.query_map(params, T::get_fn_mut)?;

        let result = rows.map(|p| p.expect("Failed to parse row")).collect();

        Ok::<Vec<T>, Box<dyn Error>>(result)
    }

    fn query_first<T: SQLModel>(
        &mut self,
        sql: sql::Select,
        params: impl Params,
    ) -> Result<Option<T>, Box<dyn Error>> {
        let mut rows = self.query(sql.limit("1"), params)?;
        Ok(rows.pop())
    }

    pub fn get_project(
        &mut self,
        project_name: &String,
    ) -> Result<Option<ProjectModel>, Box<dyn Error>> {
        self.query_first::<ProjectModel>(
            ProjectModel::sql_select().where_clause("name = $1"),
            params!(project_name),
        )
    }

    pub fn update_parser_by_id(&mut self, parser: &ParserModel) -> Result<(), Box<dyn Error>> {
        // TODO call signal
        let conn = self.get_connection();
        parser.sql_update_by_id(conn)?;
        Ok(())
    }

    pub fn get_card(&mut self, id: i32) -> Result<Option<CardModel>, Box<dyn Error>> {
        self.query_first(CardModel::sql_select().where_clause("id = $1"), params!(id))
    }

    pub fn get_parser(&mut self, id: i32) -> Result<Option<ParserModel>, Box<dyn Error>> {
        self.query_first(
            ParserModel::sql_select().where_clause("id = $1"),
            params!(id),
        )
    }

    pub fn delete_project(&mut self, project_name: &String) -> Result<(), Box<dyn Error>> {
        let sql = ProjectModel::sql_delete()
            .where_clause(format!("name = '{}'", project_name).as_str())
            .as_string();

        self.get_connection().execute(&sql, [])?;

        Ok(())
    }

    pub fn get_editor(&mut self) -> Result<EditorModel, Box<dyn Error>> {
        match self.query_first(EditorModel::sql_select(), [])? {
            Some(e) => Ok(e),
            None => {
                let editor = EditorModel {
                    last_project_name: String::from(""),
                };
                editor.sql_insert_into(self.get_connection())?;
                return Ok(editor);
            }
        }
    }

    fn get_connection(&mut self) -> &Connection {
        self.connection.get_or_init(|| {
            Connection::open(self.file_path.to_string()).expect("Failed to connect!")
        })
    }

    fn create_tables(&mut self) {
        let connection = self.get_connection();

        connection
            .execute(ProjectModel::sql_create().as_string().as_str(), [])
            .expect("Failed to create project table!");
        connection
            .execute(CardModel::sql_create().as_string().as_str(), [])
            .expect("Failed to create cards table!");
        connection
            .execute(EditorModel::sql_create().as_string().as_str(), [])
            .expect("Failed to create editors table!");
        connection
            .execute(ParserModel::sql_create().as_string().as_str(), [])
            .expect("Failed to create parsers table!");
    }

    fn drop_tables(&mut self) {
        let connection = self.get_connection();

        connection
            .execute(ParserModel::sql_drop().as_string().as_str(), [])
            .expect("Failed to drop parsers table!");
        connection
            .execute(CardModel::sql_drop().as_string().as_str(), [])
            .expect("Failed to drop cards table!");
        connection
            .execute(ProjectModel::sql_drop().as_string().as_str(), [])
            .expect("Failed to drop projects table!");
        connection
            .execute(EditorModel::sql_drop().as_string().as_str(), [])
            .expect("Failed to drop editors table!");
    }

    pub fn get_projects(&mut self) -> Result<Vec<ProjectModel>, Box<dyn Error>> {
        self.query::<ProjectModel>(ProjectModel::sql_select(), ())
    }

    pub fn get_templates(
        &mut self,
        project_name: &str,
    ) -> Result<Vec<ParserModel>, Box<dyn Error>> {
        self.query(
            ParserModel::sql_select()
                .where_clause("project_name = ?1")
                .where_clause("is_template = ?2"),
            (project_name, true),
        )
    }

    pub fn get_cards_from_project(
        &mut self,
        project_name: &str,
    ) -> Result<Vec<CardModel>, Box<dyn Error>> {
        self.query(
            CardModel::sql_select().where_clause("project_name = $1"),
            params!(project_name),
        )
    }

    pub fn insert_project(&mut self, project: &ProjectModel) -> Result<(), Box<dyn Error>> {
        project.sql_insert_into(self.get_connection())?;

        Ok(())
    }

    pub fn insert_card(&mut self, card: &CardModel) -> Result<(), Box<dyn Error>> {
        card.sql_insert_into(self.get_connection())?;

        Ok(())
    }

    pub fn update_project_description(
        &mut self,
        project_name: &String,
        new_description: &String,
    ) -> usize {
        let sql = ProjectModel::sql_update_description(new_description)
            .where_clause(format!("name = '{}'", &project_name).as_str())
            .as_string();
        self.get_connection()
            .execute(&sql, [])
            .expect("Failed to update project description")
    }

    pub fn get_parser_with_children(
        &mut self,
        parser_id: i32,
    ) -> Result<Option<ParserModel>, Box<dyn Error>> {
        let result = self.get_parser(parser_id)?;
        match result {
            None => Ok(None),
            Some(mut parser) => {
                parser.children = self.get_parser_children_rec(parser.id)?;
                Ok(Some(parser))
            }
        }
    }

    fn get_parser_children_rec(
        &mut self,
        parser_id: i32,
    ) -> Result<Vec<ParserModel>, Box<dyn Error>> {
        let children = self.query::<ParserModel>(
            ParserModel::sql_select().where_clause("parent_id = ?1"),
            params!(parser_id),
        )?;
        let mut result: Vec<ParserModel> = Vec::with_capacity(children.len());
        for mut child in children {
            // TODO check if child has is_ref flag
            child.children = self.get_parser_children_rec(child.id)?;
            result.push(child);
        }
        return Ok(result);
    }
}

#[godot_api]
impl INode for ParserRepositoryNode {
    fn ready(&mut self) {
        if self.drop_tables_on_launch {
            self.drop_tables();
        }
        self.create_tables();

        // * insert dummy data
        // self.insert_project(&ProjectModel {
        //     name: String::from("p1"),
        //     description: String::from("description"),
        // })
        // .expect("Failed to insert project");
        // self.insert_project(&ProjectModel {
        //     name: String::from("p2"),
        //     description: String::from("description"),
        // })
        // .expect("Failed to insert project");

        // let card = CardModel {
        //     name: String::from("Card1"),
        //     text: String::from("After combat: Draw 1 card."),
        //     project_name: String::from("p1"),
        // };

        // self.insert_card(&card).expect("Failed to insert card");

        // * debug print db contents
        // let projects = self.get_projects().expect("Failed to read project models");
        // godot_print!("Found {:?} projects", projects.len());
        // for project in projects {
        //     godot_print!("Found project {:?}", project);

        //     let cards = self
        //         .get_cards_from_project(&project.name)
        //         .expect(format!("Failed to get cards for project {}", &project.name).as_str());

        //     godot_print!(
        //         "Found {:?} cards for project {:?}",
        //         cards.len(),
        //         &project.name
        //     );
        //     for card in cards {
        //         godot_print!("\tFound card {:?}", card);
        //     }
        // }
    }
}

// let tx = conn.transaction()?;

//     tx.execute("delete from cat_colors", NO_PARAMS)?;
//     tx.execute("insert into cat_colors (name) values (?1)", &[&"lavender"])?;
//     tx.execute("insert into cat_colors (name) values (?1)", &[&"blue"])?;

//     tx.commit()
