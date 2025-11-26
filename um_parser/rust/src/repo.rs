use std::cell::OnceCell;
use std::error::Error;

use godot::classes::*;
use godot::prelude::*;
use rusqlite::Connection;
use rusqlite::params;

use crate::model::card::CardModel;
use crate::model::editor::EditorModel;
use crate::model::project::ProjectModel;
use crate::traits::*;

#[derive(GodotClass)]
#[class(init,base=Node)]
pub struct SQLiteParserRepository {
    #[export]
    drop_tables_on_launch: bool,

    #[export]
    file_path: GString,

    connection: OnceCell<Connection>,
}

impl SQLiteParserRepository {
    pub fn get_project(
        &mut self,
        project_name: &String,
    ) -> Result<Option<ProjectModel>, Box<dyn Error>> {
        // TODO duplicated code
        let sql = ProjectModel::sql_select()
            .where_clause(format!("name = '{}'", project_name).as_str())
            .as_string();
        let mut stmt = self.get_connection().prepare(&sql)?;

        let rows = stmt.query_map([], |row| {
            Ok(ProjectModel {
                name: row.get(0)?,
                description: row.get(1)?,
            })
        })?;
        let mut projects: Vec<ProjectModel> =
            rows.map(|p| p.expect("Failed to parse project")).collect();

        Ok(projects.pop())
    }

    pub fn get_card(
        &mut self,
        card_name: &String,
    ) -> Result<Option<CardModel>, Box<dyn Error>> {
        // TODO duplicated code
        let sql = CardModel::sql_select()
            .where_clause("name = ?1")
            .as_string();
        let mut stmt = self.get_connection().prepare(&sql)?;

        let rows = stmt.query_map(params!(card_name), |row| {
            Ok(CardModel {
                name: row.get(0)?,
                text: row.get(1)?,
                project_name: row.get(2)?,
            })
        })?;
        let mut projects: Vec<CardModel> =
            rows.map(|p| p.expect("Failed to parse card")).collect();

        Ok(projects.pop())
    }

    pub fn delete_project(&mut self, project_name: &String) -> Result<(), Box<dyn Error>> {
        let sql = ProjectModel::sql_delete()
            .where_clause(format!("name = '{}'", project_name).as_str())
            .as_string();

        self.get_connection().execute(&sql, [])?;

        Ok(())
    }

    pub fn get_editor(&mut self) -> Result<EditorModel, Box<dyn Error>> {
        let sql = EditorModel::sql_select().as_string();
        let connection = self.get_connection();
        let mut stmt = connection.prepare(&sql)?;

        let rows = stmt.query_map([], |row| {
            Ok(EditorModel {
                last_project_name: row.get(0)?,
            })
        })?;

        let mut editors: Vec<EditorModel> =
            rows.map(|p| p.expect("Failed to parse editor")).collect();

        Ok(match editors.pop() {
            Some(result) => result,
            None => {
                let editor = EditorModel {
                    last_project_name: String::from(""),
                };
                let insert_sql = editor.sql_insert().to_string();
                let _ = connection.execute(&insert_sql, [])?;
                editor
            }
        })
    }

    fn get_connection(&mut self) -> &Connection {
        self.connection.get_or_init(|| {
            Connection::open(self.file_path.to_string()).expect("Failed to connect!")
        })
    }

    fn create_tables(&mut self) {
        let connection = self.get_connection();
        godot_print!("Creating tables");

        connection
            .execute(ProjectModel::sql_create().as_string().as_str(), [])
            .expect("Failed to create project table!");
        connection
            .execute(CardModel::sql_create().as_string().as_str(), [])
            .expect("Failed to create cards table!");
        connection
            .execute(EditorModel::sql_create().as_string().as_str(), [])
            .expect("Failed to create editors table!");
        godot_print!("Tables created");
    }

    fn drop_tables(&mut self) {
        let connection = self.get_connection();
        godot_print!("Deleting tables");

        connection
            .execute(CardModel::sql_drop().as_string().as_str(), [])
            .expect("Failed to drop cards table!");
        connection
            .execute(ProjectModel::sql_drop().as_string().as_str(), [])
            .expect("Failed to drop project table!");
        connection
            .execute(EditorModel::sql_drop().as_string().as_str(), [])
            .expect("Failed to drop editor table!");
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

    pub fn get_cards_from_project(
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
        let sql = project.sql_insert().as_string();
        godot_print!("Inserting project {:?}", project);
        let _ = self.get_connection().execute(&sql, [])?;

        godot_print!("Inserted project {:?}", project);
        Ok(())
    }

    pub fn insert_card(&mut self, card: &CardModel) -> Result<(), Box<dyn Error>> {
        godot_print!("Inserting card {:?}", card);
        card.sql_insert_into(self.get_connection());

        godot_print!("Inserted card {:?}", card);
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
}

#[godot_api]
impl INode for SQLiteParserRepository {
    fn ready(&mut self) {
        godot_print!("Hello from SQLiteParserRepository");

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
