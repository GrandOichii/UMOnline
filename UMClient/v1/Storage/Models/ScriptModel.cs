using System.Collections.Generic;

public class ScriptModel : IModel
{
    public required int Id { get; set; }

    public readonly static ScriptModel Default = new()
    {
        Id = -1,  
    };

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .PrimaryKey("id")
        .Build();

    public string SQLTableName() => "scripts";


    public object[] SQLInsertData() => [
        // TODO  
    ];

    public IEnumerable<string> SQLColumns() => [
        // TODO
    ];
}