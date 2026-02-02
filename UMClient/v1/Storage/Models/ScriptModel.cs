using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using SqlKata;

public class ScriptModel : IModel
{
    public required int Id { get; set; }
    public required bool IsManual { get; set; }
    public required string ManualScript { get; set; }
    public required string GraphState { get; set; }

    public readonly static ScriptModel Default = new()
    {
        Id = -1,
        IsManual = false,
        ManualScript = "",
        GraphState = "",
    };

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("is_manual INTEGER NOT NULL")
        .Column("manual_script TEXT NOT NULL")
        .Column("graph_state TEXT NOT NULL")
        .PrimaryKey("id")
        .Build();

    public string SQLTableName() => "scripts";


    public object[] SQLInsertData() => [
        IsManual,
        ManualScript,
        GraphState,
    ];

    public IEnumerable<string> SQLColumns() => [
        "is_manual",
        "manual_script",
        "graph_state",
    ];

    public static Query SQLSelect() => new Query(Default.SQLTableName()).Select(
        "id",
        "is_manual",
        "manual_script",
        "graph_state"
    );

    public static ScriptModel SQLConverter(SqliteDataReader reader)
    {
        return new()
        {
            Id = reader.GetInt32(0),
            IsManual = reader.GetBoolean(1),
            ManualScript = reader.GetString(2),
            GraphState = reader.GetString(3),
        };
    }

    public ScriptState ParseScriptState()
    {
        return JsonSerializer.Deserialize<ScriptState>(GraphState);
    }

    public string ToScript()
    {
        if (IsManual) return ManualScript;

        // TODO
        throw new NotImplementedException();
    }

    public Query SQLDelete() => new Query(SQLTableName()).Where("id", Id).AsDelete();
}

public class ScriptState
{
    public required List<ScriptNodeState> Nodes { get; set; }
    public required List<ScriptNodeConnectionState> Connections { get; set; }

    public static ScriptState NewCardScript()
    {
        return new ScriptState()
        {
            Connections = [],
            Nodes = [
                new()
                {
                    Data = [],
                    Editor = new() { X = 0, Y = 0 },
                    Id = 0,
                    Name = "CardStart"
                }
            ],
        };
    }

    public static ScriptState NewFighterScript()
    {
        return new ScriptState()
        {
            Connections = [],
            Nodes = [
                new()
                {
                    Data = [],
                    Editor = new() { X = 0, Y = 0 },
                    Id = 0,
                    Name = "FighterStart"
                }
            ],
        };
    }

    public string ToJson() => JsonSerializer.Serialize(this);
}

public class ScriptNodeState
{
    public required int Id { get; set; }
    public required EditorState Editor { get; set; }
    public required string Name { get; set; }
    public required Dictionary<string, object> Data { get; set; }
}

public class ScriptNodeConnectionState
{
    public required int From { get; set; }
    public required int FromSlot { get; set; }
    public required int To { get; set; }
    public required int ToSlot { get; set; }
}

public class EditorState
{
    public required float X { get; set; }
    public required float Y { get; set; }
}