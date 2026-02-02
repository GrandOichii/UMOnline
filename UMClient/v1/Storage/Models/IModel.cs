using System.Collections.Generic;
using SqlKata;

public interface IModel
{
    string SQLCreate();
    string SQLTableName();
    object[] SQLInsertData();
    IEnumerable<string> SQLColumns();
    Query SQLDelete();

    string SQLDrop() => $"DROP TABLE IF EXISTS {SQLTableName()}";
    Query SQLInsert() => new Query(SQLTableName())
        .AsInsert(
            SQLColumns(),
            SQLInsertData()
        );
    Query SQLUpdate() => new Query(SQLTableName())
        .AsUpdate(
            SQLColumns(),
            SQLInsertData()
        );
}