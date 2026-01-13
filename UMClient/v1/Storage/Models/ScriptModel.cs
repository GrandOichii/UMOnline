using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SqlKata;

public class ScriptModel : IModel
{
    public required int Id { get; set; }
    public required bool IsManual { get; set; }
    public required string ManualScript { get; set; }

    public readonly static ScriptModel Default = new()
    {
        Id = -1,
        IsManual = false,
        ManualScript = "",
    };

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("is_manual INTEGER NOT NULL")
        .Column("manual_script TEXT NOT NULL")
        .PrimaryKey("id")
        .Build();

    public string SQLTableName() => "scripts";


    public object[] SQLInsertData() => [
        IsManual,
        ManualScript,
    ];

    public IEnumerable<string> SQLColumns() => [
        "is_manual",
        "manual_script",
    ];

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "id",
        "is_manual",
        "manual_script"
    );

    public static ScriptModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            Id = reader.GetInt32(0),
            IsManual = reader.GetBoolean(1),
            ManualScript = reader.GetString(2),
        };
    }
}