using System.Collections;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SqlKata;

public class DeckModel : IModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool StartsWithSidekicks { get; set; }
    // public required List<Fighter> Fighters { get; init; }
    // public required List<Card> Deck { get; init; }
    public required bool ChoosesSidekick { get; init; }
    // public required List<string> StartsWithCards { get; init; }
    // public required List<string> CantBePlayedWith { get; init; }d
    public required int? StartingHandSize { get; set; }
    public required int? MaxHandSize { get; set; }
    public required bool Editable { get; set; }
    public required string Description { get; set; }
    public required string CardBackPath { get; set; }

    public readonly static DeckModel Default = new()
    {
        ChoosesSidekick = false,
        Editable = false,
        Id = -1,
        MaxHandSize = -1,
        Name = "",
        StartingHandSize = -1,
        StartsWithSidekicks = false,
        Description = "",
        CardBackPath = "",
    };

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("name TEXT NOT NULL")
        .Column("starts_with_sidekicks INTEGER NOT NULL")
        .Column("chooses_sidekick INTEGER NOT NULL")
        .Column("starting_hand_size INTEGER")
        .Column("max_hand_size INTEGER")
        .Column("editable INTEGER NOT NULL")
        .Column("description TEXT NOT NULL")
        .Column("card_back_path TEXT")
        .PrimaryKey("id")
        .Build();

    public string SQLTableName() => "decks";

    public IEnumerable<string> SQLColumns() => [
        "name",
        "starts_with_sidekicks",
        "chooses_sidekick",
        "starting_hand_size",
        "max_hand_size",
        "editable",
        "description",
        "card_back_path",
    ];

    public object[] SQLInsertData() => [
        Name,
        StartsWithSidekicks,
        ChoosesSidekick,
        StartingHandSize,
        MaxHandSize,
        Editable,
        Description,
        CardBackPath,
    ];

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "id",
        "name",
        "starts_with_sidekicks",
        "chooses_sidekick",
        "starting_hand_size",
        "max_hand_size",
        "editable",
        "description",
        "card_back_path"
    );

    public static DeckModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            StartsWithSidekicks = reader.GetBoolean(2),
            ChoosesSidekick = reader.GetBoolean(3),
            StartingHandSize = reader.IsDBNull(4) ? null : reader.GetInt32(4),
            MaxHandSize = reader.IsDBNull(5) ? null : reader.GetInt32(5),
            Editable = reader.GetBoolean(6),
            Description = reader.GetString(7),
            CardBackPath = reader.IsDBNull(8) ? null : reader.GetString(8),
        };
    }
}