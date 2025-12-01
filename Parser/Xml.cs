
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Parser.Parsers;

namespace Parser;

static class SQL
{
    public readonly static string INSERT_FORMAT = """
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
        is_ref
    ) VALUES (
        {0},   -- id
        '{1}', -- name
        {2},   -- ptype
        '{3}', -- pattern
        '{4}', -- script
        '{5}', -- project_name
        '{6}', -- description
        {7},   -- is_template
        {8},   -- is_root
        {9},   -- parent_id
        {10}    -- is_ref
    );
    """;

    private static int _lastId = 0;
    public static int CreateId() => ++_lastId;
}


class XmlParserRoot
{
    public required string Name { get; init; }
    public XmlParserNode? Node { get; set; }

    public string ToSQLInsert(Dictionary<string, ParserBase> parserRoots)
    {
        return Node!.ToSQLInsert(parserRoots, true, Name == "root");
    }
}

class XmlParserNodeLine
{
    public required string Text { get; init; }
}

partial class XmlParserNode
{
    public required string Name { get; init; }
    public List<XmlParserNodeLine> Lines { get; } = [];
    public List<(int, XmlParserNode)> Children { get; } = [];
    public bool IsReference { get; init; } = false;

    private static string GetPattern(ParserBase parser)
    {
        return parser switch
        {
            Matcher m => m.PatternString,
            Splitter sp => sp.PatternString,
            Selector => "",
            _ => throw new Exception(),
        };
    }

    private static int GetPType(ParserBase parser)
    {
        return parser switch
        {
            Matcher => 1,
            Splitter => 2,
            Selector => 3,
            _ => throw new Exception(),
        };
    }

    public string ToSQLInsert(Dictionary<string, ParserBase> parserRoots, bool isTemplate, bool isRoot, int? parentId = null)
    {
        var id = SQL.CreateId();

        if (IsReference)
        {
            return string.Format(
                SQL.INSERT_FORMAT,
                id,
                parserRoots[Name].Name,
                GetPType(parserRoots[Name]),
                "",
                "",
                "test",
                "",
                0,
                0,
                parentId is null ? "NULL" : parentId,
                1
            );
        }

        var parser = ToParser();
        var result = new StringBuilder();
        result.AppendLine(string.Format(
            SQL.INSERT_FORMAT,
            id,
            Name,
            GetPType(),
            GetPattern(parser).Replace("{", "\\{").Replace("}", "\\}"),
            parser.Script.Replace("'", "''"),
            "test",
            "",
            isTemplate ? 1 : 0,
            isRoot ? 1 : 0,
            parentId is null ? "NULL" : parentId,
            0
        ));

        foreach (var child in GetChildren())
        {
            result.AppendLine(child.ToSQLInsert(parserRoots, false, false, id));
        }

        return result.ToString();
    }

    public int GetPType()
    {
        var matcherMatch = MatcherRegex().Match(Name);
        if (matcherMatch.Success)
        {
            return 1;
        }
        var selectorMatch = SelectorRegex().Match(Name);
        if (selectorMatch.Success)
        {
            return 2;
        }
        var splitterMatch = SplitterRegex().Match(Name);
        if (splitterMatch.Success)
        {
            return 3;
        }
        throw new Exception(Name);
    }

    public IEnumerable<XmlParserNode> GetChildren()
    {
        //         var size = Children.Max(p => p.Item1);
        //         var result = new List<XmlParserNode>(size);
        //         System.Console.WriteLine($"CAP: {result.Capacity}");
        //         foreach (var child in Children)
        //         {
        //             System.Console.WriteLine($"IDX: {child.Item1 - 1}");

        // result.
        //             result[child.Item1 - 1] = child.Item2;

        //             System.Console.WriteLine("SUCCESS");
        //         }
        // return result;
        return Children.OrderBy(p => p.Item1).Select(p => p.Item2);
    }

    public void AddChild(XmlParserNode node, string strIdx)
    {
        Children.Add((int.Parse(strIdx), node));
    }

    public ParserBase ToParserRecursive(Dictionary<string, ParserBase> parserRoots)
    {
        if (IsReference)
        {
            return parserRoots[Name];
        }

        var result = ToParser();

        foreach (var child in GetChildren())
        {
            result.Children.Add(child.ToParserRecursive(parserRoots));
        }

        return result;
    }

    public ParserBase ToParser()
    {
        var matcherMatch = MatcherRegex().Match(Name);
        if (matcherMatch.Success)
        {
            return ToMatcher(matcherMatch.Groups[1].ToString());
        }
        var selectorMatch = SelectorRegex().Match(Name);
        if (selectorMatch.Success)
        {
            return ToSelector(selectorMatch.Groups[1].ToString());
        }
        var splitterMatch = SplitterRegex().Match(Name);
        if (splitterMatch.Success)
        {
            return ToSplitter(splitterMatch.Groups[1].ToString());
        }

        throw new Exception($"Failed to parse node name: {Name}");
    }

    private static string ToScript(string text)
    {
        return $"function _Create(text, children, data) {text} end";
    }

    private Matcher ToMatcher(string name)
    {
        var pattern = Lines[0].Text.Replace("\\n", "\n").Replace("&nbsp;", " ");
        if (pattern == "-") pattern = "";
        var script = ToScript(Lines[1].Text);

        return new Matcher()
        {
            Name = name,
            Children = [],
            PatternString = pattern,
            Script = script
        };
    }

    private Selector ToSelector(string name)
    {
        if (Lines.Count > 0)
        {
            return new Selector()
            {
                Name = name,
                Children = [],
                Script = ToScript(Lines[0].Text)
            };
        }
        return new Selector()
        {
            Name = name,
            Children = []
        };
    }

    private Splitter ToSplitter(string name)
    {
        var pattern = Lines[0].Text.Replace("\\n", "\n");
        if (Lines.Count > 1)
        {
            return new Splitter()
            {
                Name = name,
                Children = [],
                PatternString = pattern,
                Script = ToScript(Lines[1].Text)
            };
        }
        return new Splitter()
        {
            Name = name,
            Children = [],
            PatternString = pattern,
        };
    }

    [GeneratedRegex("^m:(.+)$")]
    private static partial Regex MatcherRegex();
    [GeneratedRegex("^s:(.+)$")]
    private static partial Regex SelectorRegex();
    [GeneratedRegex("^sp:(.+)$")]
    private static partial Regex SplitterRegex();
}
