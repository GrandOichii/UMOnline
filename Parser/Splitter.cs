using System.Text.RegularExpressions;
using NLua;

namespace ScriptParser;

public class Splitter : ParserBase {
    public required string PatternString { 
        get => Pattern.ToString();
        set {
            Pattern = new Regex(value);
        }
    }
    public Regex Pattern { get; private set; } = null!;

    public Splitter() {
        Script = File.ReadAllText("../scripts/splitter.lua");
    }

    public override ParseResult Parse(string text)
    {
        var split = Pattern.Split(text);
        var child = Children[0];
        var status = ParseResultStatus.SUCCESS;
        var children = new List<ParseResult>();
        // var didntMatch = false;
        var failed = 0;
        foreach (var part in split) {
            if (string.IsNullOrEmpty(part)) continue;
            var partResult = child.Parse(part);
            children.Add(partResult);
            if (partResult.Status == ParseResultStatus.SUCCESS) continue;
            // if (partResult.Status == ParseResultStatus.DIDNT_MATCH) didntMatch = true;
            ++failed;
        }
        if (failed > 0) {
            status = ParseResultStatus.CHILD_FAILED;
            if (failed == split.Length) status = ParseResultStatus.ALL_CHILDREN_FAILED;
            // if (didntMatch) status = ParseResultStatus.DIDNT_MATCH;
        }
        return new(this, status, text, children);
    }

    public override string ParserType() => "splitter";
}