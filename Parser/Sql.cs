using System.Text;
using Parser.Parsers;

namespace Parser;

public class ParserModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int PType { get; set; }
    public required string Pattern { get; set; }
    public required string Script { get; set; }
    public required bool IsTemplate { get; set; }
    public required bool IsRoot { get; set; }
    public required int? ParentId { get; set; }
    public required int? RefToId { get; set; }

    public string ToSQL()
    {
        return $"""
        INSERT INTO parsers(
            id,
            name,
            ptype,
            pattern,
            script,
            project_name,
            description,
            is_template,
            is_root,
            parent_id,
            ref_to_id,
            editor_offset_x,
            editor_offset_y
        ) VALUES (
            {Id},   -- id
            '{Name}', -- name
            {PType},   -- ptype
            '{Pattern.Replace("{", "{{").Replace("}", "}}").Replace("'", "''")}', -- pattern
            '{Script.Replace("'", "''")}', -- script
            'test', -- project_name
            '', -- description
            {(IsTemplate ? 1 : 0)},   -- is_template
            {(IsRoot ? 1 : 0)},   -- is_root
            {(ParentId is null ? "NULL" : ParentId)},   -- parent_id
            {(RefToId is null ? "NULL" : RefToId)},   -- ref_to_id
            0,
            0
        );
        """;
    }
}

class IDGenerator
{
    private int _last = 0;
    public int Next() => ++_last;
}

static class ParserModelInsert
{
    public static string GenerateSQLFile(Dictionary<XmlParserRoot, ParserBase> parserRoots)
    {
        var result = new StringBuilder("delete from parsers;\n\n");
        var gen = new IDGenerator();

        // roots
        var roots = parserRoots.Select(r => r.Key.ToParserModel(gen.Next())).ToList();
        Dictionary<string, int> nameToId = roots.ToDictionary(r => r.Name, r => r.Id);
        foreach (var r in roots)
        {
            result.AppendLine(r.ToSQL());
        }

        // child nodes
        var childNodes = new List<ParserModel>();
        foreach (var pair in parserRoots)
        {
            foreach (var child in pair.Key.Node!.GetChildren())
            {
                childNodes.AddRange(child.ToParserModels(nameToId, gen, nameToId[pair.Key.Name]));
            }
        }
        foreach (var n in childNodes)
        {
            result.AppendLine(n.ToSQL());
        }

        return result.ToString();
    }
}