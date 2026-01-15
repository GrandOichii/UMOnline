use godot::classes::*;
use godot::prelude::*;

use crate::model::card::CardModel;

#[derive(GodotClass)]
#[class(init,base=Window)]
pub struct CardsManagerWindowNode {
    base: Base<Window>,

    cards: Option<Vec<CardModel>>,

    #[export_group(name = "Nodes")]
    #[export]
    used_cards_list_node: OnEditor<Gd<ItemList>>,
    #[export]
    used_cards_filter_node: OnEditor<Gd<LineEdit>>,
    #[export]
    unused_cards_list_node: OnEditor<Gd<ItemList>>,
    #[export]
    unused_cards_filter_node: OnEditor<Gd<LineEdit>>,
    #[export]
    to_used_cards_button_node: OnEditor<Gd<Button>>,
    #[export]
    to_unused_cards_button_node: OnEditor<Gd<Button>>,
    #[export]
    all_to_used_cards_button_node: OnEditor<Gd<Button>>,
    #[export]
    all_to_unused_cards_button_node: OnEditor<Gd<Button>>,
    #[export]
    selected_card_name_node: OnEditor<Gd<Label>>,
    #[export]
    selected_card_text_node: OnEditor<Gd<RichTextLabel>>,
    #[export]
    save_button_node: OnEditor<Gd<Button>>,
    #[export]
    cancel_button_node: OnEditor<Gd<Button>>,
}

#[godot_api]
impl CardsManagerWindowNode {
    #[signal]
    pub fn save_request();
    #[signal]
    pub fn cancel_request();
}

#[godot_api]
impl IWindow for CardsManagerWindowNode {
    fn ready(&mut self) {
        self.connect_signals();
    }
}

// public methods
impl CardsManagerWindowNode {
    pub fn get_card_changes(&mut self) -> (Vec<i32>, Vec<i32>) {
        // TODO only changes!
        (
            self.get_card_ids(self.used_cards_list_node.clone()),
            self.get_card_ids(self.unused_cards_list_node.clone()),
        )
    }

    pub fn load_cards(&mut self, cards: Vec<CardModel>) {
        self.used_cards_filter_node.set_text("");
        self.unused_cards_filter_node.set_text("");

        self.selected_card_name_node.set_text("");
        self.selected_card_text_node.clear();

        self.cards = Some(cards);

        self.reload_used_card_list();
        self.reload_unused_card_list();
    }
}

// private methods
impl CardsManagerWindowNode {
    fn connect_signals(&mut self) {
        self.base()
            .signals()
            .close_requested()
            .connect_other(self, Self::on_self_close_requested);
        self.cancel_button_node
            .signals()
            .pressed()
            .connect_other(self, Self::on_cancel_button_pressed);
        self.save_button_node
            .signals()
            .pressed()
            .connect_other(self, Self::on_save_button_pressed);
        self.used_cards_list_node
            .signals()
            .multi_selected()
            .connect_other(self, Self::on_used_cards_list_node_item_selected);
        self.unused_cards_list_node
            .signals()
            .multi_selected()
            .connect_other(self, Self::on_unused_cards_list_node_item_selected);
        self.used_cards_filter_node
            .signals()
            .text_changed()
            .connect_other(self, Self::on_used_cards_filter_node_text_changed);
        self.unused_cards_filter_node
            .signals()
            .text_changed()
            .connect_other(self, Self::on_unused_cards_filter_node_text_changed);
        self.to_unused_cards_button_node
            .signals()
            .pressed()
            .connect_other(self, Self::on_to_unused_cards_button_node_pressed);
        self.to_used_cards_button_node
            .signals()
            .pressed()
            .connect_other(self, Self::on_to_used_cards_button_node_pressed);
        self.all_to_unused_cards_button_node
            .signals()
            .pressed()
            .connect_other(self, Self::on_all_to_unused_cards_button_node_pressed);
        self.all_to_used_cards_button_node
            .signals()
            .pressed()
            .connect_other(self, Self::on_all_to_used_cards_button_node_pressed);
        self.used_cards_list_node
            .signals()
            .item_activated()
            .connect_other(self, Self::on_used_cards_list_node_item_activated);
        self.unused_cards_list_node
            .signals()
            .item_activated()
            .connect_other(self, Self::on_unused_cards_list_node_item_activated);
    }

    fn get_card_ids(&mut self, list: Gd<ItemList>) -> Vec<i32> {
        let mut result = vec![];
        for idx in 0..list.get_item_count() {
            result.push(list.get_item_metadata(idx).to());
        }

        result
    }

    fn reload_card_list(&mut self, mut list: Gd<ItemList>, filter: Gd<LineEdit>, used: bool) {
        let text_filter = &filter.get_text().to_lower().to_string();
        let cards: Vec<&CardModel> = self
            .cards
            .as_ref()
            .unwrap()
            .iter()
            .filter(|c| {
                c.used == used
                    && (c.name.to_lowercase().contains(text_filter)
                        || c.text.to_lowercase().contains(text_filter))
            })
            .collect();
        list.clear();
        for card in cards {
            let idx = list.add_item(&card.name);
            list.set_item_metadata(idx, &card.id.to_variant());
        }
    }

    fn reload_used_card_list(&mut self) {
        self.reload_card_list(
            self.used_cards_list_node.clone(),
            self.used_cards_filter_node.clone(),
            true,
        );
    }

    fn reload_unused_card_list(&mut self) {
        self.reload_card_list(
            self.unused_cards_list_node.clone(),
            self.unused_cards_filter_node.clone(),
            false,
        );
    }

    fn get_card(&mut self, id: i32) -> &mut CardModel {
        return self
            .cards
            .as_mut()
            .unwrap()
            .iter_mut()
            .find(|c| c.id == id)
            .unwrap();
    }

    fn display_selected_card(&mut self, list: Gd<ItemList>, idx: i32) {
        let id: i32 = list.get_item_metadata(idx).to();
        let card = self.get_card(id);

        let name = card.name.to_string();
        let text = card.text.to_string();

        self.selected_card_name_node.set_text(&name);
        self.selected_card_text_node.set_text(&text);
    }

    fn emit_close(&mut self) {
        self.signals().cancel_request().emit();
    }

    fn emit_save(&mut self) {
        self.signals().save_request().emit();
    }

    fn try_change_selected_card_states(&mut self, mut list: Gd<ItemList>, change_used_to_value: bool) {
        let selected = list.get_selected_items();
        for idx in selected.as_slice() {
            let id: i32 = list.get_item_metadata(*idx).to();
            let card: &mut CardModel = self.get_card(id);
            card.used = change_used_to_value;
        }

        self.reload_unused_card_list();
        self.reload_used_card_list();
    }

    fn try_change_all_card_states(&mut self, list: Gd<ItemList>, change_used_to_value: bool) {
        let count = list.get_item_count();
        for idx in 0..count {
            let id: i32 = list.get_item_metadata(idx).to();
            let card: &mut CardModel = self.get_card(id);
            card.used = change_used_to_value;
        }

        self.reload_unused_card_list();
        self.reload_used_card_list();
    }

    fn try_change_card_state(&mut self, list: Gd<ItemList>, idx: i32, change_used_to_value: bool) {
        let id: i32 = list.get_item_metadata(idx).to();
        let card: &mut CardModel = self.get_card(id);
        card.used = change_used_to_value;

        self.reload_unused_card_list();
        self.reload_used_card_list();
    }
}

// signal connections
impl CardsManagerWindowNode {
    fn on_self_close_requested(&mut self) {
        self.emit_close();
    }

    fn on_cancel_button_pressed(&mut self) {
        self.emit_close();
    }

    fn on_save_button_pressed(&mut self) {
        self.emit_save();
    }

    fn on_used_cards_list_node_item_selected(&mut self, idx: i64, _: bool) {
        self.display_selected_card(self.used_cards_list_node.clone(), idx.try_into().unwrap());
    }

    fn on_unused_cards_list_node_item_selected(&mut self, idx: i64, _: bool) {
        self.display_selected_card(self.unused_cards_list_node.clone(), idx.try_into().unwrap());
    }

    fn on_used_cards_filter_node_text_changed(&mut self, _: GString) {
        self.reload_used_card_list();
    }

    fn on_unused_cards_filter_node_text_changed(&mut self, _: GString) {
        self.reload_unused_card_list();
    }

    fn on_to_unused_cards_button_node_pressed(&mut self) {
        self.try_change_selected_card_states(
            self.used_cards_list_node.clone(),
            false
        )
    }
    
    fn on_to_used_cards_button_node_pressed(&mut self) {
        self.try_change_selected_card_states(
            self.unused_cards_list_node.clone(),
            true
        )
    }
    
    fn on_all_to_unused_cards_button_node_pressed(&mut self) {
        self.try_change_all_card_states(
            self.used_cards_list_node.clone(),
            false
        )
    }
    
    fn on_all_to_used_cards_button_node_pressed(&mut self) {
        self.try_change_all_card_states(
            self.unused_cards_list_node.clone(),
            true
        )
    }

    fn on_used_cards_list_node_item_activated(&mut self, idx: i64) {
        self.try_change_card_state(
            self.used_cards_list_node.clone(),
            idx.try_into().unwrap(),
            false
        )
    }

    fn on_unused_cards_list_node_item_activated(&mut self, idx: i64) {
        self.try_change_card_state(
            self.unused_cards_list_node.clone(),
            idx.try_into().unwrap(),
            true
        )
    }
}
