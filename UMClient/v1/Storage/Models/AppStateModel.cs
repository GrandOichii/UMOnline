

using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using SqlKata;

public class AppStateModel : IModel
{
    public required string LastUsedName { get; set; }
    public required string LastConnectedAddress { get; set; }
    public required DateTime? LastUpdateDT { get; set; }

    public readonly static AppStateModel Default = new()
    {
        LastUsedName = "",
        LastConnectedAddress = null,
        LastUpdateDT = null
    };

    public IEnumerable<string> SQLColumns() => [
        "last_used_name",
        "last_connected_address",
        "last_update_dt"
    ];

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("last_used_name TEXT NOT NULL")
        .Column("last_connected_address TEXT")
        .Column("last_update_dt TEXT")
        .Build();

    public object[] SQLInsertData() => [
        LastUsedName,
        LastConnectedAddress,
        LastUpdateDT
    ];

    public string SQLTableName() => "app_state";

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "last_used_name",
        "last_connected_address",
        "last_update_dt"
    );

    public static AppStateModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            LastUsedName = reader.GetString(0),
            LastConnectedAddress = reader.IsDBNull(1) ? null : reader.GetString(1),
            LastUpdateDT = reader.IsDBNull(2) ? null : DateTime.Parse(reader.GetString(2))
        };
    }
}