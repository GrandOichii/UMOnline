using System.Collections.Generic;

public class ScriptNodeModel : IModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int EditorOffsetX { get; set; }
    public required int EditorOffsetY { get; set; }
    public required int ScriptId { get; set; }

    public readonly static ScriptNodeModel Default = new()
    {
        Id = -1,
        EditorOffsetX = 0,
        EditorOffsetY = 0,
        Name = "",
        ScriptId = -1
    };

    public string SQLCreate() => new DDLCreateBuilder(SQLTableName())
        .Column("id INTEGER")
        .Column("name TEXT NOT NULL")
        .Column("editor_offset_x INTEGER NOT NULL")
        .Column("editor_offset_y INTEGER NOT NULL")
        .Column("script_id INTEGER NOT NULL")
        .PrimaryKey("id")
        .ForeignKey("script_id", ScriptModel.Default.SQLTableName(), "id")
        .Build();

    public string SQLTableName() => "script_nodes";

    
    public object[] SQLInsertData() => [
        // TODO  
    ];

    public IEnumerable<string> SQLColumns() => [
        // TODO
    ];
}