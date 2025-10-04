
using System.Text.RegularExpressions;
using System.Xml;
using Parser.Parsers;


namespace Parser;

class XmlParserRoot
{
    public required string Name { get; init; }
    public XmlParserNode? Node { get; set; }
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
