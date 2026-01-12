using System.Collections.Generic;

public class DDLCreateBuilder(string tableName)
{
    private readonly List<string> _lines = [];

    public DDLCreateBuilder Column(string column)
    {
        _lines.Add(column);
        return this;
    }
    
    public DDLCreateBuilder PrimaryKey(string column)
    {
        _lines.Add($"PRIMARY KEY ({column})");
        return this;
    }

    public DDLCreateBuilder ForeignKey(string column, string foreignTable, string foreighColumn)
    {
        _lines.Add($"FOREIGN KEY ({column}) REFERENCES {foreignTable}({foreighColumn})");
        return this;
    }

    public string Build()
    {
        var inner = string.Join(", ", _lines);
        var result = $"CREATE TABLE IF NOT EXISTS {tableName} ({inner})";
        return result;
    }
}