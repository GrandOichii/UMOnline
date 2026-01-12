using System.Collections.Generic;
using SqlKata;

public class CardModel : IModel
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required int DeckId { get; set; }

    public readonly static CardModel Default = new()
    {
        Id = -1,
        DeckId = -1,
        Title = "",
    };

    public string SQLTableName() => "cards";

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("title TEXT NOT NULL")
        .Column("deck_id INTEGER NOT NULL")
        // .PrimaryKey("id")
        .ForeignKey("deck_id", DeckModel.Default.SQLTableName(), "id")
        .Build();

    public object[] SQLInsertData() => [
        // TODO  
    ];

    public IEnumerable<string> SQLColumns() => [
        // TODO
    ];
}