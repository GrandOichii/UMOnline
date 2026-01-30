using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SqlKata;

public class CoreModel : IModel
{
    public required int Id { get; set; }
    public required string Text { get; set; }
    public required bool IsActive { get; set; }

    public readonly static CoreModel Default = new()
    {
        Id = 1,
        IsActive = false,
        Text = ""
    };



    public IEnumerable<string> SQLColumns() => [
        "text",
        "is_active",
    ];


    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("text TEXT")
        .Column("is_active INTEGER")
        .PrimaryKey("id")
        .Build();

    public object[] SQLInsertData() => [
        Text,
        IsActive,
    ];

    public string SQLTableName() => "cores";

    public static CoreModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            Id = reader.GetInt32(0),
            Text = reader.GetString(1),
            IsActive = reader.GetBoolean(2),
        };
    }

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "id",
        "text",
        "is_active"
    );

}