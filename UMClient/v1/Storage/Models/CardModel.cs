using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SqlKata;
using UMCore.Matches.Players.Cards;

public enum CardModelType
{
    Scheme = 0,
    Attack = 1,
    Defense = 2,
    Versatile = 3
}

public class CardModel : IModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Title { get; set; }
    public required int DeckId { get; set; }
    public required int Count { get; set; }
    public required int StartingHandCount { get; set; }
    public required string AllowedFighters { get; set; }
    public required string Labels { get; set; }
    public required CardModelType Type { get; set; }
    public required int Value { get; set; }
    public required int Boost { get; set; }
    public required string Text { get; set; }
    public required string ImagePath { get; set; }
    public required int ScriptId { get; set; }

    public readonly static CardModel Default = new()
    {
        Id = -1,
        DeckId = -1,
        Name = "",
        Title = "",
        AllowedFighters = "",
        Boost = -1,
        Count = -1,
        ImagePath = null,
        Labels = "",
        StartingHandCount = -1,
        Text = "",
        Type = CardModelType.Scheme,
        Value = -1,
        ScriptId = -1,
    };

    public string SQLTableName() => "cards";

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("name TEXT NOT NULL")
        .Column("title TEXT NOT NULL")
        .Column("deck_id INTEGER NOT NULL")
        .Column("count INTEGER NOT NULL")
        .Column("starting_hand_count INTEGER NOT NULL")
        .Column("allowed_fighters TEXT NOT NULL")
        .Column("labels TEXT NOT NULL")
        .Column("type INTEGER NOT NULL")
        .Column("value INTEGER NOT NULL")
        .Column("boost INTEGER NOT NULL")
        .Column("text TEXT NOT NULL")
        .Column("image_path TEXT")
        .Column("script_id INTEGER NOT NULL")
        .PrimaryKey("id")
        .ForeignKey("deck_id", DeckModel.Default.SQLTableName(), "id")
        .ForeignKey("script_id", ScriptModel.Default.SQLTableName(), "id")
        .Build();

    public object[] SQLInsertData() => [
        Name,
        Title,
        DeckId,
        Count,
        StartingHandCount,
        AllowedFighters,
        Labels,
        (int)Type,
        Value,
        Boost,
        Text,
        ImagePath,
        ScriptId,
    ];

    public IEnumerable<string> SQLColumns() => [
        "name",
        "title",
        "deck_id",
        "count",
        "starting_hand_count",
        "allowed_fighters",
        "labels",
        "type",
        "value",
        "boost",
        "text",
        "image_path",
        "script_id",
    ];

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "id",
        "name",
        "title",
        "deck_id",
        "count",
        "starting_hand_count",
        "allowed_fighters",
        "labels",
        "type",
        "value",
        "boost",
        "text",
        "image_path",
        "script_id"
    );

    public static CardModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Title = reader.GetString(2),
            DeckId = reader.GetInt32(3),
            Count = reader.GetInt32(4),
            StartingHandCount = reader.GetInt32(5),
            AllowedFighters = reader.GetString(6),
            Labels = reader.GetString(7),
            Type = (CardModelType)reader.GetInt32(8),
            Value = reader.GetInt32(9),
            Boost = reader.GetInt32(10),
            Text = reader.GetString(11),
            ImagePath = reader.IsDBNull(12) ? null : reader.GetString(12),
            ScriptId = reader.GetInt32(13),
        };
    }
}