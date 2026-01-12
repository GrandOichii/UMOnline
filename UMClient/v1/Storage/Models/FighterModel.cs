using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SqlKata;

public class FighterModel : IModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int DeckId { get; set; }
    public required int Amount { get; set; }
    public required bool IsSidekick { get; set; }
    public required bool IsSmall { get; set; }
    public required int MaxHealth { get; set; }
    public required int StartingHealth { get; set; }
    public required int MeleeRange { get; set; }
    public required bool IsRanged { get; set; }
    public required int Movement { get; set; }
    public required bool CanMoveOverOpposing { get; set; }
    public required string Text { get; set; }
    public required string ImagePath { get; set; }

    public readonly static FighterModel Default = new()
    {
        Id = -1,
        DeckId = -1,
        Name = "",
        Amount = -1,
        CanMoveOverOpposing = false,
        IsRanged = false,
        IsSidekick = false,
        IsSmall = false,
        MaxHealth = -1,
        MeleeRange = -1,
        Movement = -1,
        StartingHealth = -1,
        Text = "",
        ImagePath = null,
    };

    public string SQLTableName() => "fighters";

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("name TEXT NOT NULL")
        .Column("deck_id INTEGER NOT NULL")
        .Column("amount INTEGER NOT NULL")
        .Column("is_sidekick INTEGER NOT NULL")
        .Column("is_small INTEGER NOT NULL")
        .Column("max_health INTEGER NOT NULL")
        .Column("starting_health INTEGER NOT NULL")
        .Column("melee_range INTEGER NOT NULL")
        .Column("is_ranged INTEGER NOT NULL")
        .Column("movement INTEGER NOT NULL")
        .Column("can_move_over_opposing INTEGER NOT NULL")
        .Column("text TEXT NOT NULL")
        .Column("image_path TEXT")
        .PrimaryKey("id")
        .ForeignKey("deck_id", Default.SQLTableName(), "id")
        .Build();


    public object[] SQLInsertData() => [
        Name,
        DeckId,
        Amount,
        IsSidekick,
        IsSmall,
        MaxHealth,
        StartingHealth,
        MeleeRange,
        IsRanged,
        Movement,
        CanMoveOverOpposing,
        Text,
        ImagePath,
    ];

    public IEnumerable<string> SQLColumns() => [
        "name",
        "deck_id",
        "amount",
        "is_sidekick",
        "is_small",
        "max_health",
        "starting_health",
        "melee_range",
        "is_ranged",
        "movement",
        "can_move_over_opposing",
        "text",
        "image_path",
    ];

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "id",
        "name",
        "deck_id",
        "amount",
        "is_sidekick",
        "is_small",
        "max_health",
        "starting_health",
        "melee_range",
        "is_ranged",
        "movement",
        "can_move_over_opposing",
        "text",
        "image_path"
    );

    public static FighterModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            DeckId = reader.GetInt32(2),
            Amount = reader.GetInt32(3),
            IsSidekick = reader.GetBoolean(4),
            IsSmall = reader.GetBoolean(5),
            MaxHealth = reader.GetInt32(6),
            StartingHealth = reader.GetInt32(7),
            MeleeRange = reader.GetInt32(8),
            IsRanged = reader.GetBoolean(9),
            Movement = reader.GetInt32(10),
            CanMoveOverOpposing = reader.GetBoolean(11),
            Text = reader.GetString(12),
            ImagePath = reader.IsDBNull(13) ? null : reader.GetString(13),
        };
    }
}