using System.Collections.Generic;

public class FighterModel : IModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int DeckId { get; set; }

    public readonly static FighterModel Default = new()
    {
        Id = -1,
        DeckId = -1,
        Name = "",
    };

    public string SQLTableName() => "fighters";

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("name TEXT NOT NULL")
        .Column("deck_id INTEGER NOT NULL")
        .PrimaryKey("id")
        .ForeignKey("deck_id", DeckModel.Default.SQLTableName(), "id")
        .Build();


    public object[] SQLInsertData() => [
        // TODO  
    ];

    public IEnumerable<string> SQLColumns() => [
        // TODO
    ];
}