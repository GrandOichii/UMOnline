use std::cell::OnceCell;
use std::error::Error;

use godot::classes::*;
use godot::prelude::*;
use rusqlite::Connection;

use crate::model::card::CardModel;
use crate::model::project::ProjectModel;
use crate::traits::SQLCreate;
use crate::traits::SQLDrop;
use crate::traits::SQLInsert;
use crate::traits::SQLSelect;

pub trait ParserRepository {
    // TODO
    // fn get_projects(&mut self) -> Vec<ProjectModel>;
    // fn insert_project(&mut self, project: &ProjectModel);
    fn get_project(
        &mut self,
        project_name: &String,
    ) -> Result<Option<ProjectModel>, Box<dyn Error>>;
}

#[derive(GodotClass)]
#[class(init,base=Node)]
pub struct SQLiteParserRepository {
    #[export]
    delete_tables_on_launch: bool,

    #[export]
    file_path: GString,

    connection: OnceCell<Connection>,
}

impl ParserRepository for SQLiteParserRepository {
    fn get_project(
        &mut self,
        project_name: &String,
    ) -> Result<Option<ProjectModel>, Box<dyn Error>> {
        // TODO duplicated code
        let sql = ProjectModel::sql_select()
            .where_clause(format!("name = '{}'", project_name).as_str())
            .as_string();
        let mut stmt = self.get_connection().prepare(&sql)?;

        let project = stmt.query_one([], |row| {
            Ok(ProjectModel {
                name: row.get(0)?,
                description: row.get(1)?,
            })
        })?;

        Ok::<Option<ProjectModel>, Box<dyn Error>>(Some(project))
    }
}

impl SQLiteParserRepository {
    fn get_connection(&mut self) -> &Connection {
        self.connection.get_or_init(|| {
            Connection::open(self.file_path.to_string()).expect("Failed to connect!")
        })
    }

    fn create_tables(&mut self) {
        let connection = self.get_connection();
        godot_print!("Creating tables");

        connection
            .execute(ProjectModel::sql_create().as_str(), [])
            .expect("Failed to create project table!");
        connection
            .execute(CardModel::sql_create().as_str(), [])
            .expect("Failed to create cards table!");
        godot_print!("Tables created");
    }

    fn delete_tables(&mut self) {
        let connection = self.get_connection();
        godot_print!("Deleting tables");

        connection
            .execute(CardModel::sql_drop().as_str(), [])
            .expect("Failed to create cards table!");
        connection
            .execute(ProjectModel::sql_drop().as_str(), [])
            .expect("Failed to create project table!");
        godot_print!("Tables deleted");
    }

    pub fn get_projects(&mut self) -> Result<Vec<ProjectModel>, Box<dyn Error>> {
        let sql = ProjectModel::sql_select().as_string();
        let mut stmt = self.get_connection().prepare(&sql)?;

        let projects = stmt.query_map([], |row| {
            Ok(ProjectModel {
                name: row.get(0)?,
                description: row.get(1)?,
            })
        })?;

        let result = projects
            .map(|p| p.expect("Failed to parse project"))
            .collect();
        Ok::<Vec<ProjectModel>, Box<dyn Error>>(result)
    }

    pub fn get_cards_for_project(
        &mut self,
        project_name: &str,
    ) -> Result<Vec<CardModel>, Box<dyn Error>> {
        let sql = CardModel::sql_select()
            .where_clause(format!("project_name = '{}'", project_name).as_str())
            .as_string();
        let mut stmt = self.get_connection().prepare(&sql)?;

        let projects = stmt.query_map([], |row| {
            Ok(CardModel {
                name: row.get(0)?,
                text: row.get(1)?,
                project_name: row.get(2)?,
            })
        })?;

        let result = projects
            .map(|p| p.expect("Failed to parse project"))
            .collect();
        Ok::<Vec<CardModel>, Box<dyn Error>>(result)
    }

    pub fn insert_project(&mut self, project: &ProjectModel) -> Result<(), Box<dyn Error>> {
        let sql = project.sql_insert();
        godot_print!("Inserting project {:?}", project);
        let _ = self.get_connection().execute(&sql, [])?;

        godot_print!("Inserted project {:?}", project);
        Ok(())
    }

    pub fn insert_card(&mut self, card: &CardModel) -> Result<(), Box<dyn Error>> {
        let sql = card.sql_insert();
        godot_print!("Inserting card {:?}", card);
        let _ = self.get_connection().execute(&sql, [])?;

        godot_print!("Inserted card {:?}", card);
        Ok(())
    }
}

#[godot_api]
impl INode for SQLiteParserRepository {
    fn ready(&mut self) {
        godot_print!("Hello from SQLiteParserRepository");

        if self.delete_tables_on_launch {
            self.delete_tables();
        }
        self.create_tables();

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

        let projects = self.get_projects().expect("Failed to read project models");
        godot_print!("Found {:?} projects", projects.len());
        for project in projects {
            godot_print!("Found project {:?}", project);

            let cards = self
                .get_cards_for_project(&project.name)
                .expect(format!("Failed to get cards for project {}", &project.name).as_str());

            godot_print!(
                "Found {:?} cards for project {:?}",
                cards.len(),
                &project.name
            );
            for card in cards {
                godot_print!("\tFound card {:?}", card);
            }
        }
    }
}

// impl ParserRepository for SQLiteParserRepository {

// }
