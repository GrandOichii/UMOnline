using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SqlKata;

public class CoreModel : IModel
{
    public required int Id { get; set; }
    public required string Text { get; set; }

    public readonly static CoreModel Default = new()
    {
        Id = 1,
        Text = ""
    };


    public IEnumerable<string> SQLColumns() => [
        "text",
    ];


    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("text TEXT")
        .PrimaryKey("id")
        .Build();

    public object[] SQLInsertData() => [
        Text,
    ];

    public string SQLTableName() => "core";

    public static CoreModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            Id = reader.GetInt32(0),
            Text = reader.GetString(1),
        };
    }

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "id",
        "text"
    );

    public Query SQLDelete() => throw new Exception("Cannot delete core");

}