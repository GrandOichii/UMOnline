### Match
+ CurPlayerIdx: int
- Map: Map
+ Players: Player[]
- NewEvents: Event[]
- NewLogs: Log[]
- Combat: Combat?

    ### Event
    - PlayedBy: string
    - Type: scheme|attack

        ### SchemeEvent : Event
        - CardPlayed: string

        ### CombatEvent
        - AttackerPlayerIdx: int
        - DefenderPlayerIdx: int
        - Attacker: CombatCard
        - Defender: CombatCard?
        - DamageDealt: int?

            ### CombatCard
            - Card: string?
            - BoostCards: string?[]
            - Value: int?

    ### Log
    - Message: string
    - PlayerIdx: int

    ### Combat
    - Event: CombatEvent

    ### Player
    + Idx: int
    + Actions: int
    + Deck: CardCollection
    + Hand: CardCollection
    + Discard: CardCollection
    + Fighters: Fighter[]

        ### CardCollection
        + Cards: string?[]
        + Count: int

        ### Fighter
        + Id: int
        + Name: string
        + IsAlive: bool
        + CurHealth: int
        + MaxHealth: int

    ### Map
    - Nodes: Node[]

        ### Node
        - Id: int
        - FighterId: int?