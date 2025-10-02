### Effects
- Do X. If Y, do Z instead
    - Your opponent discards a card. If you won the combat, they discard 2 instead.
    - Your opponent puts 1 random card from their hand on top of their deck. If Eredin is ENRAGED, they discard it instead
    - Deal 1 damage to each opposing fighter that is adjacent to at least 1 of your fighters. If Eredin is ENRAGED, deal 2 damage instead
- A may B. If C, D (already exists, modify to accept this as well)
    - Your opponent may discard a card. If they don't, return a defeated Archer to a space in Yennenga's zone

### Simple effect selector:
- This cards value is +X for Y
    - This card's value is +1 for each Sister in the same zone as the opposing fighter
    - This card's value is +1 for each other VOYAGE card in your discard pile
    - +1 to this attack for each trap token adjacent to the opposing fighter
    - Add +1 to this card's value for each other friendly fighter adjacent to the opposing fighter
    - This card's value is +1 for each Rage you have
    - For each zone the opposing fighter is in, increase the value of this card by +1
    - This card's value is +1 for each Hellfire you have
    - Increase the value of this card by +1 for each squirrel adjacent to the opposing fighter
- Do X actions. This cards value is +Y for each action
    - During combat: You may discard any number of cards from your hand. This card's value is +1 for each card you discard
    - You may discard Dr{DOTSPACE}Jekyll cards. Add 2 to this card's value for each card discarded
    - Spend any amount of Rage. This card's value is +2 for each Rage spent.
    - You may spend any amount of Hellfire. Increase the value of this card by that amount
- All of X (attacks|defences|cards) this turn are +X value
    - All of Bruce Lee's attacks this turn are +1 value
- Choose and look. Discard
    - Choose an opponent and look at their hand. Choose a card for them to discard
- Cancel any abilities of opponent's card
    - Cancel any abilities on your opponent's card
- Move TO
    - Move Willow to any space in her zone
    - Move to any space in your zone // ? assert that player has a single fighter
- Return cards from discard to hand
    - Take all other VOYAGE cards from your discard pile and add them to your hand
    - Choose an attack, defense, or versatile card from your discard pile and return it to your hand
    - Choose a card in your discard pile and return it to your hand
- You may defeat X to Y
    - You may defeat a Red Rider to return this card to your hand
- Shuffle cards from discard back into deck
    - choose 2 cards in your discard pile and shuffle them into your deck
- TAKING damage
    - all opposing fighters on spaces with fog tokens take 1 damage
- Additional attack markers
    - TRICKED YOU
- Additional attacks
    - TRICKED YOU: 4 ATK
    - BLOODY REPRISE: 0 ATK
    - RELENTLESS ASSAULT: 3 ATK
- Card value modification
    - The value of this card is equal to the printed value of your opponent's card

### Little Red
- Do X. $BASKET$ Instead, do Y
    - You may return this card to your hand.\n$ROSE$ Instead, you may return the top card of your discard pile to your hand.
- Do X. $BASKET$ Also Y
    - Cancel all effects on your opponent's card. \n$ROSE$ Also ignore the value of your opponent's card
- Do X. $BASKET$ Y instead.
    - Little Red Recovers 2 health.\n$PELT$ Little Red recovers 4 health instead.

### Static
- This card's effects cannot be canceled
- Modifying card type rules
    - $SWORDS$ You may play this card as a defense card
    - you may play this card face-up to target a fighter in any space

### Mission (check - are they just conditions?)